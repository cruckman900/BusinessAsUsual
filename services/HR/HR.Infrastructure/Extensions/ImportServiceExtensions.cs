using HR.Domain.Entities;
using HR.Infrastructure.Persistence;
using Platform.Infrastructure.Services;

namespace HR.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering HR entities with Platform import services
/// </summary>
public static class ImportServiceExtensions
{
    /// <summary>
    /// Registers HR tables and entities for import functionality
    /// </summary>
    public static void RegisterHRTablesForImport()
    {
        // Register the HR DbContext with table names
        SchemaIntrospectionService.RegisterDbContext<HRDbContext>("Employee", "Department");

        // Register entity types
        SchemaIntrospectionService.RegisterEntityType("Employee", typeof(Employee).FullName!);
        SchemaIntrospectionService.RegisterEntityType("Department", typeof(Department).FullName!);

        // Additional HR entities can be registered here as needed
    }
}
