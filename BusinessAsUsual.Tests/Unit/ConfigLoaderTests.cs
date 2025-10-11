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
        /// Verifies that <see cref="ConfigLoader.LoadEnvironmentVariables()"/> loads AWS_SQL_CONNECTION_STRING correctly.
        /// </summary>
        [Fact]
        public void ShouldLoadEnvironmentVariablesFromDotEnv()
        {
            // Act
            ConfigLoader.LoadEnvironmentVariables(); // Let it auto-resolve path

            // Assert
            var connectionString = Environment.GetEnvironmentVariable("AWS_SQL_CONNECTION_STRING");

            Assert.False(string.IsNullOrWhiteSpace(connectionString));
            Assert.Contains("Server=", connectionString);
            Assert.Contains("Database=", connectionString);
            Assert.Contains("User Id=", connectionString);
            Assert.Contains("Password=", connectionString);
        }
    }
}