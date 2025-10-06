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
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantMetadataService"/> class
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public TenantMetadataService(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Saves metadata for a newly provisioned company.
        /// </summary>
        /// <param name="company">The company metadata to sae.</param>
        public async Task SaveAsync(Company company)
        {
            var connStr = _config.GetConnectionString("MasterAdmin");
            
            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                INSERT INTO Companies (Id, Name, DbName, AdminEmail, CreatedAt)
                VALUES (@Id, @Name, @DbName, @AdminEmail, @CreatedAt)", conn);

            cmd.Parameters.AddWithValue("@Id", company.Id);
            cmd.Parameters.AddWithValue("@Name", company.Name);
            cmd.Parameters.AddWithValue("@DbName", company.DbName);
            cmd.Parameters.AddWithValue("@AdminEmail", company.AdminEmail);
            cmd.Parameters.AddWithValue("@CreatedAt", company.CreatedAt);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
