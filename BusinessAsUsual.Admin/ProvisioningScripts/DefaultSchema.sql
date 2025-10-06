-- Default schema for new company database
CREATE TABLE CompanyInfo (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100),
    CreatedAt DATETIME
);

CREATE TABLE Employees (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER,
    Name NVARCHAR(100),
    Role NVARCHAR(50),
    Email NVARCHAR(100),
    CreatedAt DATETIME
);

CREATE TABLE Products (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER,
    Name NVARCHAR(100),
    Description NVARCHAR(255),
    Price DECIMAL(18,2),
    CreatedAt DATETIME
);

-- Optional: stored procedures, triggers, views
-- CREATE PROCEDURE sp_AddEmployee AS ...