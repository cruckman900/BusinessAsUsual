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
    Description NVARCHAR(MAX),
    Price DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE Services (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE Customers (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    MiddleName NVARCHAR(100),
    Prefix NVARCHAR(50),
    Suffix NVARCHAR(50),
    Email NVARCHAR(100),
    Phone NVARCHAR(50),
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE Orders (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    DiscountAmount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    TaxAmount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    ShippingAmount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    FulfillmentStatus NVARCHAR(50) NOT NULL DEFAULT 'Unfulfilled',
    Total DECIMAL(18,2) NOT NULL DEFAULT 0.00
);

CREATE TABLE OrderItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    OrderId UNIQUEIDENTIFIER NOT NULL,
    ItemType NVARCHAR(50) NOT NULL, -- 'Product' or 'Service'
    ItemId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    UnitPrice DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    DiscountAmount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    TaxAmount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    LineTotal DECIMAL(18,2) NOT NULL
);

CREATE TABLE Fulfillment (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    OrderId UNIQUEIDENTIFIER NOT NULL,
    FulfilledAt DATETIME,
    FulfilledBy NVARCHAR(100),
    Notes NVARCHAR(MAX)
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

ALTER TABLE Customers
ADD CONSTRAINT FK_Customers_CompanyInfo
FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(id);

ALTER TABLE Orders
ADD CONSTRAINT FK_Orders_CompanyInfo
FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);

ALTER TABLE Orders
ADD CONSTRAINT FK_Orders_Customers
FOREIGN KEY (CustomerId) REFERENCES Customers(Id);

ALTER TABLE OrderItems
ADD CONSTRAINT FK_OrderItems_CompanyInfo
FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);

ALTER TABLE OrderItems
ADD CONSTRAINT FK_OrderItems_Orders
FOREIGN KEY (OrderId) REFERENCES Orders(Id);

ALTER TABLE Fulfillment
ADD CONSTRAINT FK_Fulfillment_CompanyInfo
FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);

ALTER TABLE Fulfillment
ADD CONSTRAINT FK_Fulfillment_Orders
FOREIGN KEY (OrderId) REFERENCES Orders(Id);

CREATE INDEX IX_CompanySettings_CompanyId ON CompanySettings(CompanyId);
CREATE INDEX IX_CompanyLocation_CompanyId ON CompanyLocation(CompanyId);
CREATE INDEX IX_BillingHistory_CompanyId ON BillingHistory(CompanyId);
CREATE INDEX IX_ModuleUsage_CompanyId ON ModuleUsage(CompanyId);
CREATE INDEX IX_AuditLog_CompanyId ON AuditLog(CompanyId);
CREATE INDEX IX_ProvisioningLog_CompanyId ON ProvisioningLog(CompanyId);
CREATE INDEX IX_Employees_CompanyId ON Employees(CompanyId);
CREATE INDEX IX_Products_CompanyId ON Products(CompanyId);
CREATE INDEX IX_Services_CompanyId ON Services(CompanyId);
CREATE INDEX IX_Customers_CompanyId ON Customers(CompanyId);
CREATE INDEX IX_Orders_CompanyId ON Orders(CompanyId);
CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);
CREATE INDEX IX_OrderItems_CompanyId ON OrderItems(CompanyId);
CREATE INDEX IX_OrderItems_OrderId ON OrderItems(OrderId);
CREATE INDEX IX_Fulfillment_CompanyId ON Fulfillment(CompanyId);
CREATE INDEX IX_Fulfillment_OrderId ON Fulfillment(OrderId);

CREATE UNIQUE INDEX UX_ModuleUsage_Company_Module ON ModuleUsage(CompanyId, ModuleName);
CREATE UNIQUE INDEX UX_CompanySettings_CompanyId ON CompanySettings(CompanyId);

-- Repeat for other tables as needed

-- Optional: stored procedures, triggers, views
-- CREATE PROCEDURE sp_AddEmployee AS ...