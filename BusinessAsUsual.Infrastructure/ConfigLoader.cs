using System;
using System.IO;

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
            return Environment.GetEnvironmentVariable(key)
                ?? throw new InvalidOperationException($"Missing config value for {key}");
        }

        /// <summary>
        /// Loads environment variables from the appropriate .env file based on environment context.
        /// </summary>
        public static void LoadEnvironmentVariables()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            string envFileName = environment switch
            {
                "Production" => ".env.production",
                "CI" => ".env.ci",
                _ => ".env.local"
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
                envFilePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", envFileName));
            }

            if (File.Exists(envFilePath))
            {
                Console.WriteLine($"✅ ConfigLoader: Loading {envFileName} from {envFilePath}");
                LoadEnvironmentVariables(envFilePath);
            }
            else
            {
                Console.WriteLine($"⚠️ ConfigLoader: {envFileName} not found at {envFilePath}");
            }
        }

        /// <summary>
        /// Loads environment variables from a specified .env file path.
        /// </summary>
        public static void LoadEnvironmentVariables(string envFilePath)
        {
            if (!File.Exists(envFilePath))
            {
                Console.WriteLine($"⚠️ ConfigLoader: No .env file found at {envFilePath}");
                return;
            }

            var lines = File.ReadAllLines(envFilePath);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#'))
                    continue;

                var parts = trimmed.Split('=', 2);
                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim().Trim('"');

                Environment.SetEnvironmentVariable(key, value);
                Console.WriteLine($"🔧 ConfigLoader: Loaded SA_PASSWORD from {Path.GetFileName(envFilePath)}");
            }
        }
    }
}