using System;
using System.IO;
using System.Text.Json;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace HR.Infrastructure;

/// <summary>
/// Loads environment variables from a centralized .env file and system overrides.
/// Ensures contributor clarity and legacy-minded configuration.
/// </summary>
public static class ConfigLoader
{
    /// <summary>
    /// Gets a required environment variable by key.
    /// </summary>
    public static string Get(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrWhiteSpace(value))
        {
            Console.WriteLine($"⚠️ ConfigLoader: Missing config value for {key}");
            return ""; // or throw if truly unrecoverable
        }
        return value;
    }

    private static string GetProjectRoot()
    {
        var envOverride = Environment.GetEnvironmentVariable("PROJECT_ROOT");
        Console.WriteLine($"PROJECT_ROOT env: {envOverride}");

        if (!string.IsNullOrEmpty(envOverride))
            return envOverride;

        var currentDir = Directory.GetCurrentDirectory();
        var directoryInfo = new DirectoryInfo(currentDir);
        while (directoryInfo != null && !File.Exists(Path.Combine(directoryInfo.FullName, "BusinessAsUsual.sln")))
        {
            directoryInfo = directoryInfo.Parent;
        }
        if (directoryInfo == null)
        {
            throw new InvalidOperationException("Could not find project root directory.");
        }
        return directoryInfo.FullName;
    }

    /// <summary>
    /// Loads environment variables from the appropriate .env file based on environment context.
    /// </summary>
    public static async Task LoadEnvironmentVariables()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        Console.WriteLine($"🌍 ASPNETCORE_ENVIRONMENT = {environment}");

        // 1. Load from .env file only if NOT production
        if (!environment.Equals("Production", StringComparison.OrdinalIgnoreCase))
        {
            string envFileName = environment switch
            {
                "Development" => ".env.local",
                "CI" => ".env.ci",
                _ => ".env.ci"
            };

            string envFilePath;

            if (Directory.Exists("/app"))
            {
                // Running inside Docker
                envFilePath = Path.Combine("/app", envFileName);
            }
            else
            {
                // Running locally (e.g., Test Explorer, CLI)
                envFilePath = Path.Combine(GetProjectRoot(), envFileName);
            }

            if (environment == "CI" && Directory.Exists("/app"))
            {
                Console.WriteLine("ℹ️ CI mode inside Docker: ignoring .env.ci");
                return;
            }

            if (File.Exists(envFilePath))
            {
                Console.WriteLine($"✅ ConfigLoader: Loading {envFileName} from {envFilePath}");
                await LoadEnvironmentVariables(envFilePath);
            }
            else
            {
                Console.WriteLine($"⚠️ ConfigLoader: {envFileName} not found at {envFilePath}");
            }
        }
        else
        {
            Console.WriteLine("ℹ️ ConfigLoader: Skipping .env loading in Production environment.");

            // 2. Attempt AWS Secrets Manager
            try
            {
                var client = new AmazonSecretsManagerClient();
                var request = new GetSecretValueRequest
                {
                    SecretId = "BusinessAsUsual/Secrets"
                };

                var response = await client.GetSecretValueAsync(request);
                var secrets = JsonSerializer.Deserialize<Dictionary<string, string>>(response.SecretString);

                var safeSecrets = secrets ?? new Dictionary<string, string>();

                foreach (var kvp in safeSecrets
                    .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value)))
                {
                    Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
                    Console.WriteLine($"🔐 ConfigLoader: Loaded {kvp.Key} from AWS Secrets Manager");
                }

                Console.WriteLine("✅ ConfigLoader: Loaded secrets from AWS Secrets Manager");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ ConfigLoader: Failed to load from AWS Secrets Manager: {ex.Message}");
                Console.WriteLine("ℹ️ ConfigLoader: Falling back to environment variables.");
            }
        }
    }

    private static async Task LoadEnvironmentVariables(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"⚠️ ConfigLoader: File not found: {filePath}");
            return;
        }

        var lines = await File.ReadAllLinesAsync(filePath);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                continue;

            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                var value = parts[1].Trim().Trim('"');
                Environment.SetEnvironmentVariable(key, value);
                Console.WriteLine($"✅ ConfigLoader: Set {key}");
            }
        }
    }
}
