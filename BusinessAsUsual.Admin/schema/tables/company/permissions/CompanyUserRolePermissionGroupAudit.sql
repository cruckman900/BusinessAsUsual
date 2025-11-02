CREATE TABLE CompanyUserRolePermissionGroupAudit (
    AuditID CHAR(36) NOT NULL PRIMARY KEY,
    CompanyID CHAR(36) NOT NULL,
    PermissionGroupID CHAR(36) NOT NULL,
    ChangedByUserID CHAR(36) NOT NULL,
    ChangeType VARCHAR(50) NOT NULL,                      -- e.g., "AddedMember", "RemovedMember", "AssignedToRole", "RevokedFromRole"
    ChangeDetails TEXT,                                   -- Optional JSON or notes
    ChangeTimestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    INDEX idx_CURPGA_CompanyID (CompanyID),
    INDEX idx_CURPGA_PermissionGroupID (PermissionGroupID),
    INDEX idx_CURPGA_ChangedByUserID (ChangedByUserID)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;