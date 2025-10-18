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
        MiddleName NVARCHAR(100) NOT NULL,
        Prefix NVARCHAR(50) NOT NULL,
        Suffix NVARCHAR(50) NOT NULL,
        Role NVARCHAR(50),
        Email NVARCHAR(100),
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        CompanyId UNIQUEIDENTIFIER NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(MAX),
        Price DECIMAL(18,2) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Services')
BEGIN
    CREATE TABLE Services (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        CompanyId UNIQUEIDENTIFIER NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(MAX),
        Price DECIMAL(18,2) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
BEGIN
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
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
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
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderItems')
BEGIN
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
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Fulfillment')
BEGIN
    CREATE TABLE Fulfillment (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        CompanyId UNIQUEIDENTIFIER NOT NULL,
        OrderId UNIQUEIDENTIFIER NOT NULL,
        FulfilledAt DATETIME,
        FulfilledBy NVARCHAR(100),
        Notes NVARCHAR(MAX)
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
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_Products_CompanyInfo'
)
BEGIN
    ALTER TABLE Products
    ADD CONSTRAINT FK_Products_CompanyInfo
    FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_Services_CompanyInfo'
)
BEGIN
    ALTER TABLE Services
    ADD CONSTRAINT FK_Services_CompanyInfo
    FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_Customers_CompanyInfo'
)
BEGIN
    ALTER TABLE Customers
    ADD CONSTRAINT FK_Customers_CompanyInfo
    FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_CompanyInfo'
)
BEGIN
    ALTER TABLE Orders
    ADD CONSTRAINT FK_Orders_CompanyInfo
    FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_Customers'
)
BEGIN
    ALTER TABLE Orders
    ADD CONSTRAINT FK_Orders_Customers
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderItems_CompanyInfo'
)
BEGIN
    ALTER TABLE OrderItems
    ADD CONSTRAINT FK_OrderItems_CompanyInfo
    FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderItems_Orders'
)
BEGIN
    ALTER TABLE OrderItems
    ADD CONSTRAINT FK_OrderItems_Orders
    FOREIGN KEY (OrderId) REFERENCES Orders(Id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_Fulfillment_CompanyInfo'
)
BEGIN
    ALTER TABLE Fulfillment
    ADD CONSTRAINT FK_Fulfillment_CompanyInfo
    FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_Fulfillment_Orders'
)
BEGIN
    ALTER TABLE Fulfillment
    ADD CONSTRAINT FK_Fulfillment_Orders
    FOREIGN KEY (OrderId) REFERENCES Orders(Id);
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
    SELECT * FROM sys.indexes WHERE name = 'IX_Products_CompanyId'
)
BEGIN
    CREATE INDEX IX_Products_CompanyId ON Products(CompanyId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_Services_CompanyId'
)
BEGIN
    CREATE INDEX IX_Services_CompanyId ON Services(CompanyId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_Customers_CompanyId'
)
BEGIN
    CREATE INDEX IX_Customers_CompanyId ON Customers(CompanyId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_Orders_CompanyId'
)
BEGIN
    CREATE INDEX IX_Orders_CompanyId ON Orders(CompanyId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_Orders_CustomerId'
)
BEGIN
    CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_OrderItems_CompanyId'
)
BEGIN
    CREATE INDEX IX_OrderItems_CompanyId ON OrderItems(CompanyId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_OrderItems_OrderId'
)
BEGIN
    CREATE INDEX IX_OrderItems_OrderId ON OrderItems(OrderId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_Fulfillment_CompanyId'
)
BEGIN
    CREATE INDEX IX_Fulfillment_CompanyId ON Fulfillment(CompanyId);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes WHERE name = 'IX_Fulfillment_OrderId'
)
BEGIN
    CREATE INDEX IX_Fulfillment_OrderId ON Fulfillment(OrderId);
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