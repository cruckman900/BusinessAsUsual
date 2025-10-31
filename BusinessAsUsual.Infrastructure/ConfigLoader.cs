using System;
using System.IO;
using System.Text.Json;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace BusinessAsUsual.Infrastructure
{
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
                        Console.WriteLine($"🔐 ConfigLoader: Loaded {kvp.Key}={kvp.Value} from AWS Secrets Manager");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ ConfigLoader: Failed to load secrets from AWS — {ex.Message}");
                }
            }

            Console.WriteLine("✅ ConfigLoader: Environment loading complete.");
        }

        /// <summary>
        /// Loads environment variables from a specified .env file path.
        /// </summary>
        public static async Task LoadEnvironmentVariables(string envFilePath)
        {
            var loadedKeys = new HashSet<string>();

            // 1. Load from .env.local if present
            if (File.Exists(envFilePath))
            {
                var lines = await File.ReadAllLinesAsync(envFilePath);
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#')) continue;

                    var parts = trimmed.Split('=', 2);
                    if (parts.Length != 2) continue;

                    var key = parts[0].Trim();
                    var value = parts[1].Trim().Trim('"');

                    Environment.SetEnvironmentVariable(key, value);
                    loadedKeys.Add(key);
                    Console.WriteLine($"🔧 ConfigLoader: Loaded {key} from {Path.GetFileName(envFilePath)}");
                }
            }
            else
            {
                Console.WriteLine($"⚠️ ConfigLoader: No .env file found at {envFilePath}");
            }

            // 2. Load from AWS Secrets Manager if available
            try
            {
                var client = new AmazonSecretsManagerClient();
                var request = new GetSecretValueRequest
                {
                    SecretId = "BusinessAsUsual/Secrets"
                };

                var response = await client.GetSecretValueAsync(request);
                var secrets = JsonSerializer.Deserialize<Dictionary<string, string>>(response.SecretString);

                if (secrets != null)
                {
                    foreach (var kvp in secrets
                        .Where(kvp => !loadedKeys.Contains(kvp.Key) && !string.IsNullOrWhiteSpace(kvp.Value)))
                    {
                        Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
                        Console.WriteLine($"🔐 ConfigLoader: Loaded {kvp.Key} from AWS Secrets Manager");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ ConfigLoader: Failed to load secrets from AWS — {ex.Message}");
            }

            // 3. Log final state for contributor clarity
            Console.WriteLine("✅ ConfigLoader: Environment variables loaded. Keys:");
            foreach (var key in loadedKeys)
            {
                Console.WriteLine($"   - {key}");
            }
        }

        /// <summary>
        /// Attempts to retrieve a secret value from AWS Secrets Manager.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Secret Value for Key</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<string> GetSecretValue(string key)
        {
            var client = new AmazonSecretsManagerClient();
            var request = new GetSecretValueRequest
            {
                SecretId = "BusinessAsUsual/Secrets"
            };

            var response = await client.GetSecretValueAsync(request);
            var secrets = JsonSerializer.Deserialize<Dictionary<string, string>>(response.SecretString);

            if (secrets != null && secrets.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            throw new InvalidOperationException($"Missing or empty secret value for key: {key}");
        }
    }
}