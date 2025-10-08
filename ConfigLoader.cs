using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using DotNetEnv;
using System.Configuration;
using System.Runtime.InteropServices;

namespace BusinessAsUsual.Infrastructure.Config;

public static class ConfigLoader
{
    public static IConfiguration Load(string envFileName = ".env.aws")
    {
        var rootPath = Directory.GetParent(AppContext.BaseDirectory)?.FullName ?? AppContext.BaseDirectory;
        var envPath = Path.Combine(rootPath, envFileName);

        if (File.Exists(envPath))
        {
            Env.Load(envPath);
            Console.WriteLine($"✅ Loaded environment variables from: {envFileName}");
        }
        else
        {
            Console.WriteLine($"⚠️ Environment file not found: {envFileName}. Skipping .env loading.");
        }

        var config = new ConfigurationBuilder()
            .SetBasePath(rootPath)
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        return config;
    }
}