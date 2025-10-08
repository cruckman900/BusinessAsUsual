-- Creates the global Companies table if it doesn't already exist
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
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
END