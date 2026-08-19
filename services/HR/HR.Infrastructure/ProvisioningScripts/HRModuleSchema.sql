-- ============================================================
-- HR MODULE PROVISIONING SCRIPT
-- ============================================================
-- This script creates the HR module's database schema for a tenant.
-- It is executed on-demand when the HR module is first accessed (lazy-loading).
-- All tables include CompanyId for multi-tenancy support.
-- ============================================================

-- HR_Employees Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HR_Employees')
BEGIN
	CREATE TABLE HR_Employees (
		Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
		CompanyId UNIQUEIDENTIFIER NOT NULL,
		FirstName NVARCHAR(100) NOT NULL,
		LastName NVARCHAR(100) NOT NULL,
		MiddleName NVARCHAR(100),
		Email NVARCHAR(255) NOT NULL,
		PersonalEmail NVARCHAR(255),
		PhoneNumber NVARCHAR(20),
		JobTitle NVARCHAR(100),
		WorkLocation NVARCHAR(100),
		SalaryGrade NVARCHAR(50),
		DateOfBirth DATE,
		HireDate DATE,
		TerminationDate DATE,
		TerminationReason NVARCHAR(500),
		Status NVARCHAR(50) NOT NULL DEFAULT 'Active',
		EmploymentType NVARCHAR(50) NOT NULL DEFAULT 'FullTime',
		ManagerId UNIQUEIDENTIFIER,
		AddressLine1 NVARCHAR(200),
		AddressLine2 NVARCHAR(200),
		City NVARCHAR(100),
		State NVARCHAR(100),
		PostalCode NVARCHAR(20),
		Country NVARCHAR(100),
		EmergencyContactName NVARCHAR(100),
		EmergencyContactPhone NVARCHAR(20),
		EmergencyContactRelationship NVARCHAR(50),
		Department NVARCHAR(100),
		CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
		UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),

		-- Foreign key to self for Manager relationship
		CONSTRAINT FK_HR_Employees_Manager FOREIGN KEY (ManagerId) REFERENCES HR_Employees(Id)
	);

	CREATE INDEX IX_HR_Employees_CompanyId ON HR_Employees(CompanyId);
	CREATE UNIQUE INDEX IX_HR_Employees_Email ON HR_Employees(Email);
	CREATE INDEX IX_HR_Employees_Status ON HR_Employees(Status);
	CREATE INDEX IX_HR_Employees_ManagerId ON HR_Employees(ManagerId);
END
GO

-- HR_Departments Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HR_Departments')
BEGIN
	CREATE TABLE HR_Departments (
		Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
		CompanyId UNIQUEIDENTIFIER NOT NULL,
		Name NVARCHAR(100) NOT NULL,
		Description NVARCHAR(500),
		Code NVARCHAR(20),
		Location NVARCHAR(100),
		CostCenter NVARCHAR(50),
		ParentDepartmentId UNIQUEIDENTIFIER,
		ManagerEmployeeId UNIQUEIDENTIFIER,
		IsActive BIT NOT NULL DEFAULT 1,
		CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
		UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),

		-- Self-referencing foreign key for department hierarchy
		CONSTRAINT FK_HR_Departments_Parent FOREIGN KEY (ParentDepartmentId) REFERENCES HR_Departments(Id),
		-- Foreign key to HR_Employees for legacy manager
		CONSTRAINT FK_HR_Departments_Manager FOREIGN KEY (ManagerEmployeeId) REFERENCES HR_Employees(Id)
	);

	CREATE INDEX IX_HR_Departments_CompanyId ON HR_Departments(CompanyId);
	CREATE INDEX IX_HR_Departments_Name ON HR_Departments(Name);
	CREATE INDEX IX_HR_Departments_Code ON HR_Departments(Code);
	CREATE INDEX IX_HR_Departments_ParentDepartmentId ON HR_Departments(ParentDepartmentId);
END
GO

-- HR_EmployeeDepartments Junction Table (many-to-many)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HR_EmployeeDepartments')
BEGIN
	CREATE TABLE HR_EmployeeDepartments (
		EmployeeId UNIQUEIDENTIFIER NOT NULL,
		DepartmentId UNIQUEIDENTIFIER NOT NULL,
		CompanyId UNIQUEIDENTIFIER NOT NULL,
		IsPrimary BIT NOT NULL DEFAULT 0,
		JoinedDate DATE NOT NULL DEFAULT CAST(GETUTCDATE() AS DATE),
		LeftDate DATE,

		PRIMARY KEY (EmployeeId, DepartmentId),
		CONSTRAINT FK_HR_EmployeeDepartments_Employee FOREIGN KEY (EmployeeId) REFERENCES HR_Employees(Id) ON DELETE CASCADE,
		CONSTRAINT FK_HR_EmployeeDepartments_Department FOREIGN KEY (DepartmentId) REFERENCES HR_Departments(Id) ON DELETE CASCADE
	);

	CREATE INDEX IX_HR_EmployeeDepartments_CompanyId ON HR_EmployeeDepartments(CompanyId);
	CREATE INDEX IX_HR_EmployeeDepartments_IsPrimary ON HR_EmployeeDepartments(IsPrimary);
	CREATE INDEX IX_HR_EmployeeDepartments_LeftDate ON HR_EmployeeDepartments(LeftDate);
END
GO

-- HR_DepartmentManagers Junction Table (many-to-many)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HR_DepartmentManagers')
BEGIN
	CREATE TABLE HR_DepartmentManagers (
		DepartmentId UNIQUEIDENTIFIER NOT NULL,
		ManagerId UNIQUEIDENTIFIER NOT NULL,
		CompanyId UNIQUEIDENTIFIER NOT NULL,
		IsPrimary BIT NOT NULL DEFAULT 0,
		ManagerRole NVARCHAR(100),
		StartDate DATE NOT NULL DEFAULT CAST(GETUTCDATE() AS DATE),
		EndDate DATE,

		PRIMARY KEY (DepartmentId, ManagerId),
		CONSTRAINT FK_HR_DepartmentManagers_Department FOREIGN KEY (DepartmentId) REFERENCES HR_Departments(Id) ON DELETE CASCADE,
		CONSTRAINT FK_HR_DepartmentManagers_Manager FOREIGN KEY (ManagerId) REFERENCES HR_Employees(Id) ON DELETE CASCADE
	);

	CREATE INDEX IX_HR_DepartmentManagers_CompanyId ON HR_DepartmentManagers(CompanyId);
	CREATE INDEX IX_HR_DepartmentManagers_IsPrimary ON HR_DepartmentManagers(IsPrimary);
	CREATE INDEX IX_HR_DepartmentManagers_EndDate ON HR_DepartmentManagers(EndDate);
END
GO

-- HR_TrainingCompletions Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HR_TrainingCompletions')
BEGIN
	CREATE TABLE HR_TrainingCompletions (
		Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
		CompanyId UNIQUEIDENTIFIER NOT NULL,
		EmployeeId UNIQUEIDENTIFIER NOT NULL,
		CourseName NVARCHAR(200) NOT NULL,
		CertificateNumber NVARCHAR(100),
		CompletionDate DATE NOT NULL,
		ExpiryDate DATE,
		RecordedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),

		CONSTRAINT FK_HR_TrainingCompletions_Employee FOREIGN KEY (EmployeeId) REFERENCES HR_Employees(Id) ON DELETE CASCADE
	);

	CREATE INDEX IX_HR_TrainingCompletions_CompanyId ON HR_TrainingCompletions(CompanyId);
	CREATE INDEX IX_HR_TrainingCompletions_EmployeeId ON HR_TrainingCompletions(EmployeeId);
	CREATE INDEX IX_HR_TrainingCompletions_CompletionDate ON HR_TrainingCompletions(CompletionDate);
END
GO

-- HR_PerformanceReviews Table (additional submodule)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HR_PerformanceReviews')
BEGIN
	CREATE TABLE HR_PerformanceReviews (
		Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
		CompanyId UNIQUEIDENTIFIER NOT NULL,
		EmployeeId UNIQUEIDENTIFIER NOT NULL,
		ReviewerId UNIQUEIDENTIFIER,
		ReviewPeriodStart DATE NOT NULL,
		ReviewPeriodEnd DATE NOT NULL,
		ReviewDate DATE NOT NULL,
		OverallRating INT,
		Comments NVARCHAR(MAX),
		Goals NVARCHAR(MAX),
		Status NVARCHAR(50) NOT NULL DEFAULT 'Draft',
		CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
		UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),

		CONSTRAINT FK_HR_PerformanceReviews_Employee FOREIGN KEY (EmployeeId) REFERENCES HR_Employees(Id) ON DELETE CASCADE,
		CONSTRAINT FK_HR_PerformanceReviews_Reviewer FOREIGN KEY (ReviewerId) REFERENCES HR_Employees(Id)
	);

	CREATE INDEX IX_HR_PerformanceReviews_CompanyId ON HR_PerformanceReviews(CompanyId);
	CREATE INDEX IX_HR_PerformanceReviews_EmployeeId ON HR_PerformanceReviews(EmployeeId);
	CREATE INDEX IX_HR_PerformanceReviews_ReviewDate ON HR_PerformanceReviews(ReviewDate);
	CREATE INDEX IX_HR_PerformanceReviews_Status ON HR_PerformanceReviews(Status);
END
GO

PRINT '✅ HR Module schema successfully provisioned';
