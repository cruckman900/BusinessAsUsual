CREATE TABLE IF NOT EXISTS CompanyUserRolePermissionOverrides (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    CompanyUserRolePermissionId CHAR(36) NOT NULL,
    OverrideKey VARCHAR(100) NOT NULL,
    OverrideValue TEXT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;