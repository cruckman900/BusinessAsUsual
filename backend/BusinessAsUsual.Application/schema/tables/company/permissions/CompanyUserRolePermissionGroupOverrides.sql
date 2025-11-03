CREATE TABLE CompanyUserRolePermissionGroupOverrides (
    OverrideID CHAR(36) NOT NULL PRIMARY KEY,
    CompanyID CHAR(36) NOT NULL,
    PermissionGroupID CHAR(36) NOT NULL,
    RoleID CHAR(36) NOT NULL,
    PermissionID CHAR(36) NOT NULL,
    IsGranted BOOLEAN NOT NULL,
    OverriddenByUserID CHAR(36) NOT NULL,
    OverrideReason TEXT,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    INDEX idx_CURPGO_CompanyID (CompanyID),
    INDEX idx_CURPGO_PermissionGroupID (PermissionGroupID),
    INDEX idx_CURPGO_RoleID (RoleID),
    INDEX idx_CURPGO_PermissionID (PermissionID),
    INDEX idx_CURPGO_OverriddenByUserID (OverriddenByUserID)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;