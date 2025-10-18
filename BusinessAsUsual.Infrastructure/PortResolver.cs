using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAsUsual.Infrastructure
{
    public static class PortResolver
    {
        public static string GetSqlConnectionString(string dbName)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var raw = Environment.GetEnvironmentVariable("AWS_SQL_CONNECTION_STRING");

            if (string.IsNullOrWhiteSpace(raw))
                throw new InvalidOperationException("Missing AWS_SQL_CONNECTION_STRING");

            var builder = new SqlConnectionStringBuilder(raw);

            // CI override: use dynamic port if available
            if (env == "CI")
            {
                var ciPort = Environment.GetEnvironmentVariable("CI_SQL_PORT");
                if (!string.IsNullOrWhiteSpace(ciPort))
                    builder.DataSource = $"localhost,{ciPort}";
            }

            builder.InitialCatalog = dbName;
            return builder.ConnectionString;
        }
    }
}
