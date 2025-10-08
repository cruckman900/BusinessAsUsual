-- Default schema for new company database
CREATE TABLE CompanyInfo (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME NOT NULL
);

CREATE TABLE CompanySettings (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    BillingPlan NVARCHAR(50) NOT NULL,
    ModulesEnabled NVARCHAR(MAX) NOT NULL,
    IsDisabled BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL
);

CREATE TABLE BillingHistory (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Description NVARCHAR(255),
    BilledAt DATETIME NOT NULL
);

CREATE TABLE ModuleUsage (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    ModuleName NVARCHAR(100) NOT NULL,
    UsageCount INT NOT NULL DEFAULT 0,
    LastUsed DATETIME NOT NULL
);

CREATE TABLE AuditLog (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    Actor NVARCHAR(100) NOT NULL,
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
    Name NVARCHAR(100) NOT NULL,
    Role NVARCHAR(50),
    Email NVARCHAR(100),
    CreatedAt DATETIME NOT NULL
);

CREATE TABLE Products (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    Price DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME NOT NULL
);

ALTER TABLE CompanySettings
ADD CONSTRAINT FK_CompanySettings_CompanyInfo
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

CREATE INDEX IX_CompanySettings_CompanyId ON CompanySettings(CompanyId);
CREATE INDEX IX_BillingHistory_CompanyId ON BillingHistory(CompanyId);
CREATE INDEX IX_ModuleUsage_CompanyId ON ModuleUsage(CompanyId);
CREATE INDEX IX_AuditLog_CompanyId ON AuditLog(CompanyId);
CREATE INDEX IX_ProvisioningLog_CompanyId ON ProvisioningLog(CompanyId);
CREATE INDEX IX_Employees_CompanyId ON Employees(CompanyId);
CREATE INDEX IX_Products_CompanyId ON Products(CompanyId);
-- Repeat for other tables as needed

-- Optional: stored procedures, triggers, views
-- CREATE PROCEDURE sp_AddEmployee AS ...