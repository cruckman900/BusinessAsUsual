CREATE TABLE CompanyUserRolePermissionGroupMembers (
    MemberID CHAR(36) NOT NULL PRIMARY KEY,
    CompanyID CHAR(36) NOT NULL,
    PermissionGroupID CHAR(36) NOT NULL,
    PermissionID CHAR(36) NOT NULL,
    AddedByUserID CHAR(36) NOT NULL,
    AddedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Notes TEXT,

    INDEX idx_CURPGM_CompanyID (CompanyID),
    INDEX idx_CURPGM_PermissionGroupID (PermissionGroupID),
    INDEX idx_CURPGM_PermissionID (PermissionID),
    INDEX idx_CURPGM_AddedByUserID (AddedByUserID)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;