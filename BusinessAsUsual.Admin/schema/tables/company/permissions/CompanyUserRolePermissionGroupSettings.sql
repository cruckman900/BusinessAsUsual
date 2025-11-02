CREATE TABLE CompanyUserRolePermissionGroupSettings (
    PermissionGroupSettingID CHAR(36) NOT NULL PRIMARY KEY,
    CompanyID CHAR(36) NOT NULL,
    PermissionGroupID CHAR(36) NOT NULL,
    SettingKey VARCHAR(100) NOT NULL,
    SettingValue TEXT,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT NULL,

    INDEX idx_CURPGS_CompanyID (CompanyID),
    INDEX idx_CURPGS_PermissionGroupID (PermissionGroupID),
    INDEX idx_CURPGS_IsActive (IsActive),
    INDEX idx_CURPGS_SettingKey (SettingKey)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;