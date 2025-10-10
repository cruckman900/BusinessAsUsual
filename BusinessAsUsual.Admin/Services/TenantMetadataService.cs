using BusinessAsUsual.Admin.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace BusinessAsUsual.Admin.Services
{
    /// <summary>
    /// Handles saving and retrieving metadata for provisioned companies.
    /// </summary>
    public class TenantMetadataService
    {
        /// <summary>
        /// If not exists, Create Companies table in the BusinessAsUsual database.
        /// </summary>
        /// <returns>Sql</returns>
        public virtual string GetCompanyRegistryScript()
        {
            return @"
            IF NOT EXISTS (
                SELECT * FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_NAME = 'Companies'
            )
            BEGIN
                CREATE TABLE Companies (
                    Id UNIQUEIDENTIFIER PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL,
                    DbName NVARCHAR(100) NOT NULL,
                    AdminEmail NVARCHAR(100) NOT NULL,
                    BillingPlan NVARCHAR(50) NOT NULL DEFAULT 'Standard',
                    ModulesEnabled NVARCHAR(MAX) NOT NULL DEFAULT 'Employees,Products',
                    IsDisabled BIT NOT NULL DEFAULT 0,
                    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
                );
            END";
        }

        /// <summary>
        /// Returns the SQL script to create the ProvisioningLog in BusinessAsUsual.
        /// </summary>
        /// <returns>Sql</returns>
        public virtual string GetProvisioningLogScript()
        {
            return $@"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProvisioningLog')
            BEGIN
                CREATE TABLE ProvisioningLog (
                    Id INT PRIMARY KEY IDENTITY,
                    TenantName NVARCHAR(100),
                    Step NVARCHAR(50),
                    Status NVARCHAR(50),
                    Message NVARCHAR(MAX),
                    Timestamp DATETIME
                );
            END";
        }

        /// <summary>
        /// Returns the SQL script to create metadata tables for tenant
        /// </summary>
        /// <param name="env"></param>
        /// <param name="tenantName"></param>
        /// <returns></returns>
        public virtual string GetCreateScript(IHostEnvironment env, string tenantName)
        {
            var path = Path.Combine(env.ContentRootPath, "ProvisioningScripts", "DefaultSchema.sql");
            return File.ReadAllText(path);
        }
    }
}
