using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence;

/// <summary>
/// HR Database Context
/// </summary>
public class HRDbContext : DbContext
{
    public HRDbContext(DbContextOptions<HRDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<EmployeeDepartment> EmployeeDepartments => Set<EmployeeDepartment>();
    public DbSet<DepartmentManager> DepartmentManagers => Set<DepartmentManager>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Employee Configuration
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PersonalEmail).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.JobTitle).HasMaxLength(100);
            entity.Property(e => e.WorkLocation).HasMaxLength(100);
            entity.Property(e => e.SalaryGrade).HasMaxLength(50);
            entity.Property(e => e.AddressLine1).HasMaxLength(200);
            entity.Property(e => e.AddressLine2).HasMaxLength(200);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.EmergencyContactName).HasMaxLength(100);
            entity.Property(e => e.EmergencyContactPhone).HasMaxLength(20);
            entity.Property(e => e.EmergencyContactRelationship).HasMaxLength(50);
            entity.Property(e => e.TerminationReason).HasMaxLength(500);

#pragma warning disable CS0618 // Type or member is obsolete - configuring legacy Department field for backward compatibility
            // Legacy department field - keep for backward compatibility
            entity.Property(e => e.Department).HasMaxLength(100);
#pragma warning restore CS0618

            entity.Property(e => e.Status).IsRequired().HasConversion<string>();
            entity.Property(e => e.EmploymentType).IsRequired().HasConversion<string>();

            entity.HasIndex(e => e.Email).IsUnique();

            // Self-referencing relationship for Manager-DirectReports
            entity.HasOne(e => e.Manager)
                  .WithMany(e => e.DirectReports)
                  .HasForeignKey(e => e.ManagerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Department Configuration
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
            entity.Property(d => d.Description).HasMaxLength(500);
            entity.Property(d => d.Code).HasMaxLength(20);
            entity.Property(d => d.Location).HasMaxLength(100);
            entity.Property(d => d.CostCenter).HasMaxLength(50);

            entity.HasIndex(d => d.Name);
            entity.HasIndex(d => d.Code);

            // Self-referencing relationship for Parent-SubDepartments (hierarchy)
            entity.HasOne(d => d.ParentDepartment)
                  .WithMany(d => d.SubDepartments)
                  .HasForeignKey(d => d.ParentDepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);

#pragma warning disable CS0618 // Type or member is obsolete - configuring legacy Manager relationship for backward compatibility
            // Legacy single manager relationship - keep for backward compatibility
            entity.HasOne(d => d.Manager)
                  .WithMany()
                  .HasForeignKey(d => d.ManagerEmployeeId)
                  .OnDelete(DeleteBehavior.SetNull);
#pragma warning restore CS0618
        });

        // EmployeeDepartment Junction Configuration
        modelBuilder.Entity<EmployeeDepartment>(entity =>
        {
            entity.HasKey(ed => new { ed.EmployeeId, ed.DepartmentId });

            entity.HasOne(ed => ed.Employee)
                  .WithMany(e => e.EmployeeDepartments)
                  .HasForeignKey(ed => ed.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ed => ed.Department)
                  .WithMany(d => d.EmployeeDepartments)
                  .HasForeignKey(ed => ed.DepartmentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(ed => ed.IsPrimary);
            entity.HasIndex(ed => ed.LeftDate);
        });

        // DepartmentManager Junction Configuration
        modelBuilder.Entity<DepartmentManager>(entity =>
        {
            entity.HasKey(dm => new { dm.DepartmentId, dm.ManagerId });

            entity.HasOne(dm => dm.Department)
                  .WithMany(d => d.DepartmentManagers)
                  .HasForeignKey(dm => dm.DepartmentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(dm => dm.Manager)
                  .WithMany(e => e.ManagedDepartments)
                  .HasForeignKey(dm => dm.ManagerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(dm => dm.ManagerRole).HasMaxLength(100);
            entity.HasIndex(dm => dm.IsPrimary);
            entity.HasIndex(dm => dm.EndDate);
        });
    }
}
