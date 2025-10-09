-- Default schema for new company database
CREATE TABLE CompanyInfo (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE CompanySettings (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    BillingPlan NVARCHAR(50) NOT NULL,
    ModulesEnabled NVARCHAR(MAX) NOT NULL,
    IsDisabled BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

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
    ShipCountry NVARCHAR(100),
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE BillingHistory (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Description NVARCHAR(255) NOT NULL,
    BilledAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    PaidOn DATETIME
);

CREATE TABLE ModuleUsage (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    ModuleName NVARCHAR(100) NOT NULL,
    UsageCount INT NOT NULL DEFAULT 0,
    LastUsed DATETIME NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE AuditLog (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    Actor NVARCHAR(100) NOT NULL,
    Action NVARCHAR(255) NOT NULL,
    EntityId UNIQUEIDENTIFIER,
    Timestamp DATETIME NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE ProvisioningLog (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    Step NVARCHAR(100),
    Status NVARCHAR(50),
    Message NVARCHAR(255),
    Timestamp DATETIME NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE Employees (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    MiddleName NVARCHAR(100) NOT NULL,
    Prefix NVARCHAR(50) NOT NULL,
    Suffix NVARCHAR(50) NOT NULL,
    Role NVARCHAR(50),
    Email NVARCHAR(100),
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE Products (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    Price DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE Services (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    Price DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

ALTER TABLE CompanySettings
ADD CONSTRAINT FK_CompanySettings_CompanyInfo
FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);

ALTER TABLE CompanyLocation
ADD CONSTRAINT FK_CompanyLocation_CompanyInfo
FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);

ALTER TABLE BillingHistory
ADD CONSTRAINT FK_BillingHistory_CompanyInfo
FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);

ALTER TABLE ModuleUsage
ADD CONSTRAINT FK_ModuleUsage_CompanyInfo
FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);

ALTER TABLE AuditLog
ADD CONSTRAINT FK_AuditLog_CompanyInfo
FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);

ALTER TABLE ProvisioningLog
ADD CONSTRAINT FK_ProvisioningLog_CompanyInfo
FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);

ALTER TABLE Employees
ADD CONSTRAINT FK_Employees_CompanyInfo
FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);

ALTER TABLE Products
ADD CONSTRAINT FK_Products_CompanyInfo
FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);

ALTER TABLE Services
ADD CONSTRAINT FK_Services_CompanyInfo
FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);

CREATE INDEX IX_CompanySettings_CompanyId ON CompanySettings(CompanyId);
CREATE INDEX IX_CompanyLocation_CompanyId ON CompanyLocation(CompanyId);
CREATE INDEX IX_BillingHistory_CompanyId ON BillingHistory(CompanyId);
CREATE INDEX IX_ModuleUsage_CompanyId ON ModuleUsage(CompanyId);
CREATE INDEX IX_AuditLog_CompanyId ON AuditLog(CompanyId);
CREATE INDEX IX_ProvisioningLog_CompanyId ON ProvisioningLog(CompanyId);
CREATE INDEX IX_Employees_CompanyId ON Employees(CompanyId);
CREATE INDEX IX_Products_CompanyId ON Products(CompanyId);
CREATE INDEX IX_Services_CompanyId ON Services(CompanyId);

CREATE UNIQUE INDEX UX_ModuleUsage_Company_Module ON ModuleUsage(CompanyId, ModuleName);
CREATE UNIQUE INDEX UX_CompanySettings_CompanyId ON CompanySettings(CompanyId);

-- Repeat for other tables as needed

-- Optional: stored procedures, triggers, views
-- CREATE PROCEDURE sp_AddEmployee AS ...