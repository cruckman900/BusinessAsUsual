CREATE TABLE CompanyUserRolePermissionAudit (
    AuditID CHAR(36) NOT NULL PRIMARY KEY,
    CompanyID CHAR(36) NOT NULL,
    RoleID CHAR(36) NOT NULL,
    PermissionID CHAR(36) NOT NULL,
    ChangedByUserID CHAR(36) NOT NULL,
    ChangeType VARCHAR(50) NOT NULL,                      -- e.g., "Granted", "Revoked", "Overridden"
    ChangeTimestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Notes TEXT,

    INDEX idx_CURPA_CompanyID (CompanyID),
    INDEX idx_CURPA_RoleID (RoleID),
    INDEX idx_CURPA_PermissionID (PermissionID),
    INDEX idx_CURPA_ChangedByUserID (ChangedByUserID)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;