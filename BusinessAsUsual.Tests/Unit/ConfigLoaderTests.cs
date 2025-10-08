using Xunit;
using BusinessAsUsual.Infrastructure;
using System;

namespace BusinessAsUsual.Tests.Infrastructure
{
    /// <summary>
    /// Unit tests for the <see cref="ConfigLoader"/> class.
    /// Verifies that environment variables are correctly loaded from a .env file.
    /// </summary>
    public class ConfigLoaderTests
    {
        /// <summary>
        /// Ensures that <see cref="ConfigLoader.LoadEnvironmentVariables"/> successfully loads
        /// the <c>DB_CONNECTION_STRING</c> from the .env file and that it contains expected components.
        /// </summary>
        [Fact]
        public void ShouldLoadEnvironmentVariablesFromDotEnv()
        {
            // Act
            ConfigLoader.LoadEnvironmentVariables();

            // Assert
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            Assert.False(string.IsNullOrWhiteSpace(connectionString));
            Assert.Contains("Server=", connectionString);
            Assert.Contains("Database=", connectionString);
            Assert.Contains("User Id=", connectionString);
            Assert.Contains("Password=", connectionString);
        }
    }
}