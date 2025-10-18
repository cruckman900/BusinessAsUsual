CREATE PROCEDURE UpdateSchemaForAllCompanies
AS
BEGIN
    SET NOCOUNT ON;

    -- Add column to CompanyInfo if missing
    IF NOT EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'CompanyInfo' AND COLUMN_NAME = 'UpdatedAt'
    )
    BEGIN
        ALTER TABLE CompanyInfo ADD UpdatedAt DATETIME NULL;
        PRINT '🧱 Added UpdatedAt to CompanyInfo';
    END

    -- Add column to CompanySettings if missing
    IF NOT EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'CompanySettings' AND COLUMN_NAME = 'LastModifiedBy'
    )
    BEGIN
        ALTER TABLE CompanySettings ADD LastModifiedBy NVARCHAR(100) NULL;
        PRINT '🧱 Added LastModifiedBy to CompanySettings';
    END

    -- Add new index if missing
    IF NOT EXISTS (
        SELECT * FROM sys.indexes WHERE name = 'IX_CompanySettings_LastModifiedBy'
    )
    BEGIN
        CREATE INDEX IX_CompanySettings_LastModifiedBy ON CompanySettings(LastModifiedBy);
        PRINT '⚡ Created index IX_CompanySettings_LastModifiedBy';
    END

    -- Add new FK if needed
    IF NOT EXISTS (
        SELECT * FROM sys.foreign_keys WHERE name = 'FK_CompanySettings_ModifierUser'
    )
    BEGIN
        ALTER TABLE CompanySettings
        ADD CONSTRAINT FK_CompanySettings_ModifierUser
        FOREIGN KEY (LastModifiedBy) REFERENCES Users(Id);
        PRINT '🔗 Added FK_CompanySettings_ModifierUser';
    END
END
