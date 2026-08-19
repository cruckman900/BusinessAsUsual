-- Default schema for new company database
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CompanyInfo')
BEGIN
    CREATE TABLE CompanyInfo (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CompanySettings')
BEGIN
    CREATE TABLE CompanySettings (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        CompanyId UNIQUEIDENTIFIER NOT NULL,
        BillingPlan NVARCHAR(50) NOT NULL,
        ModulesEnabled NVARCHAR(MAX) NOT NULL,
        IsDisabled BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CompanyLocation')
BEGIN
    CREATE TABLE CompanyLocation (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        CompanyId UNIQUEIDENTIFIER NOT NULL,
        LocationName NVARCHAR(100) NOT NULL,
        ContactName NVARCHAR(100) NOT NULL,
        ContactEmail NVARCHAR(100) NOT NULL,
        BillAddr1 NVARCHAR(100) NOT NULL,
        BillAddr2 NVARCHAR(100),
        BillCity NVARCHAR(100) NOT NULL,
        BillState NVARCHAR(50) NOT NULL,
        BillZipcode NVARCHAR(25) NOT NULL,
        BillCountry NVARCHAR(100) NOT NULL,
        ShipAddr1 NVARCHAR(100),
        ShipAddr2 NVARCHAR(100),
        ShipCity NVARCHAR(100),
        ShipState NVARCHAR(50),
        ShipCountry NVARCHAR(100),
        ShipZipcode NVARCHAR(25),
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BillingHistory')
BEGIN
    CREATE TABLE BillingHistory (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        CompanyId UNIQUEIDENTIFIER NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        Description NVARCHAR(255) NOT NULL,
        BilledAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
        PaidOn DATETIME
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ModuleUsage')
BEGIN
    CREATE TABLE ModuleUsage (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        CompanyId UNIQUEIDENTIFIER NOT NULL,
        ModuleName NVARCHAR(100) NOT NULL,
        UsageCount INT NOT NULL DEFAULT 0,
        LastUsed DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AuditLog')
BEGIN
    CREATE TABLE AuditLog (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        CompanyId UNIQUEIDENTIFIER NOT NULL,
        Actor NVARCHAR(100) NOT NULL,
        Action NVARCHAR(255) NOT NULL,
        EntityId UNIQUEIDENTIFIER,
        Timestamp DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Employees')
BEGIN
    CREATE TABLE Employees (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        CompanyId UNIQUEIDENTIFIER NOT NULL,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        MiddleName NVARCHAR(100),
        Prefix NVARCHAR(50),
        Suffix NVARCHAR(50),
        Email NVARCHAR(255),
        PhoneNumber NVARCHAR(25),
        Department NVARCHAR(100),
        JobTitle NVARCHAR(100),
        DateOfBirth DATE,
        HireDate DATE,
        EmploymentType NVARCHAR(50),
        Status NVARCHAR(50) NOT NULL DEFAULT 'Active',
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        CompanyId UNIQUEIDENTIFIER NOT NULL,
        Email NVARCHAR(255) NOT NULL,
        FirstName NVARCHAR(100),
        LastName NVARCHAR(100),
        Role NVARCHAR(50) NOT NULL DEFAULT 'User',
        ExternalAuthProvider NVARCHAR(50),
        ExternalAuthId NVARCHAR(255),
        IsActive BIT NOT NULL DEFAULT 1,
        LastLoginAt DATETIME,
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
BEGIN
    CREATE TABLE Roles (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        CompanyId UNIQUEIDENTIFIER NOT NULL,
        RoleName NVARCHAR(100) NOT NULL,
        Permissions NVARCHAR(MAX),
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ModuleRegistry')
BEGIN
    CREATE TABLE ModuleRegistry (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        CompanyId UNIQUEIDENTIFIER NOT NULL,
        ModuleConfiguration NVARCHAR(MAX) NOT NULL,
        UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ApiKeys')
BEGIN
    CREATE TABLE ApiKeys (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        CompanyId UNIQUEIDENTIFIER NOT NULL,
        KeyName NVARCHAR(100) NOT NULL,
        KeyHash NVARCHAR(MAX) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
        ExpiresAt DATETIME,
        LastUsedAt DATETIME,
        IsActive BIT NOT NULL DEFAULT 1
    );
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_CompanySettings_CompanyInfo'
)
BEGIN
    ALTER TABLE CompanySettings
    ADD CONSTRAINT FK_CompanySettings_CompanyInfo
    FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_CompanyLocation_CompanyInfo'
)
BEGIN
    ALTER TABLE CompanyLocation
    ADD CONSTRAINT FK_CompanyLocation_CompanyInfo
    FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_BillingHistory_CompanyInfo'
)
BEGIN
    ALTER TABLE BillingHistory
    ADD CONSTRAINT FK_BillingHistory_CompanyInfo
    FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_ModuleUsage_CompanyInfo'
)
BEGIN
    ALTER TABLE ModuleUsage
    ADD CONSTRAINT FK_ModuleUsage_CompanyInfo
    FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_AuditLog_CompanyInfo'
)
BEGIN
    ALTER TABLE AuditLog
    ADD CONSTRAINT FK_AuditLog_CompanyInfo
    FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_Employees_CompanyInfo'
)
BEGIN
    ALTER TABLE Employees
    ADD CONSTRAINT FK_Employees_CompanyInfo
    FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_Users_CompanyInfo'
)
BEGIN
    ALTER TABLE Users
    ADD CONSTRAINT FK_Users_CompanyInfo
    FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_Roles_CompanyInfo'
)
BEGIN
    ALTER TABLE Roles
    ADD CONSTRAINT FK_Roles_CompanyInfo
    FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_ModuleRegistry_CompanyInfo'
)
BEGIN
    ALTER TABLE ModuleRegistry
    ADD CONSTRAINT FK_ModuleRegistry_CompanyInfo
    FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_ApiKeys_CompanyInfo'
)
BEGIN
    ALTER TABLE ApiKeys
    ADD CONSTRAINT FK_ApiKeys_CompanyInfo
    FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_CompanySettings_CompanyId'
)
BEGIN
    CREATE INDEX IX_CompanySettings_CompanyId ON CompanySettings(CompanyId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_CompanyLocation_CompanyId'
)
BEGIN
    CREATE INDEX IX_CompanyLocation_CompanyId ON CompanyLocation(CompanyId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_BillingHistory_CompanyId'
)
BEGIN
    CREATE INDEX IX_BillingHistory_CompanyId ON BillingHistory(CompanyId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_ModuleUsage_CompanyId'
)
BEGIN
    CREATE INDEX IX_ModuleUsage_CompanyId ON ModuleUsage(CompanyId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_AuditLog_CompanyId'
)
BEGIN
    CREATE INDEX IX_AuditLog_CompanyId ON AuditLog(CompanyId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_Employees_CompanyId'
)
BEGIN
    CREATE INDEX IX_Employees_CompanyId ON Employees(CompanyId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_Users_CompanyId'
)
BEGIN
    CREATE INDEX IX_Users_CompanyId ON Users(CompanyId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email'
)
BEGIN
    CREATE INDEX IX_Users_Email ON Users(Email);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_Roles_CompanyId'
)
BEGIN
    CREATE INDEX IX_Roles_CompanyId ON Roles(CompanyId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_ModuleRegistry_CompanyId'
)
BEGIN
    CREATE UNIQUE INDEX IX_ModuleRegistry_CompanyId ON ModuleRegistry(CompanyId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_ApiKeys_CompanyId'
)
BEGIN
    CREATE INDEX IX_ApiKeys_CompanyId ON ApiKeys(CompanyId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'UX_ModuleUsage_Company_Module'
)
BEGIN
    CREATE UNIQUE INDEX UX_ModuleUsage_Company_Module ON ModuleUsage(CompanyId, ModuleName);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'UX_CompanySettings_CompanyId'
)
BEGIN
    CREATE UNIQUE INDEX UX_CompanySettings_CompanyId ON CompanySettings(CompanyId);
END
GO

-- Optional: stored procedures, triggers, views
-- CREATE PROCEDURE sp_AddEmployee AS ...