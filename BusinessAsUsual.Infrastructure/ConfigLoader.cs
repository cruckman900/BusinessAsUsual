using System;
using System.IO;
using System.Collections.Generic;

namespace BusinessAsUsual.Infrastructure
{
    /// <summary>
    /// Loads environment variables from .env files and system overrides.
    /// Ensures contributor clarity and legacy-minded configuration.
    /// </summary>
    public static class ConfigLoader
    {
        /// <summary>
        /// Loads environment variables from the specified .env file.
        /// </summary>
        /// <param name="envFilePath">Path to the .env file (default: ".env")</param>
        public static void LoadEnvironmentVariables(string envFilePath = ".env")
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

                // Skip comments and empty lines
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#'))
                    continue;

                var parts = trimmed.Split('=', 2);
                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim().Trim('"');

                // Only set if not already defined
                if (Environment.GetEnvironmentVariable(key) == null)
                {
                    Environment.SetEnvironmentVariable(key, value);
                    Console.WriteLine($"🔧 ConfigLoader: Loaded {key} from .env");
                }
            }
        }
    }
}