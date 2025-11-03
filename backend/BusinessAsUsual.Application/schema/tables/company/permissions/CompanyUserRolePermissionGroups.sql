CREATE TABLE CompanyUserRolePermissionGroups (
    PermissionGroupID CHAR(36) NOT NULL PRIMARY KEY,
    CompanyID CHAR(36) NOT NULL,
    GroupName VARCHAR(100) NOT NULL,
    Description TEXT,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedByUserID CHAR(36) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT NULL,

    INDEX idx_CURPG_CompanyID (CompanyID),
    INDEX idx_CURPG_GroupName (GroupName),
    INDEX idx_CURPG_IsActive (IsActive),
    INDEX idx_CURPG_CreatedByUserID (CreatedByUserID)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;