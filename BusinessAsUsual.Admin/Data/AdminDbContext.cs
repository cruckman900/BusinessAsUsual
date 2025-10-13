using BusinessAsUsual.Admin.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessAsUsual.Admin.Data
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options)
            : base(options) { }

        public DbSet<Company> Companies { get; set; }
    }
}