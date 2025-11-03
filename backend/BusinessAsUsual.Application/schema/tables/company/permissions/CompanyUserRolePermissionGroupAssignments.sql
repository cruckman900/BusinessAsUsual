CREATE TABLE CompanyUserRolePermissionGroupAssignments (
    AssignmentID CHAR(36) NOT NULL PRIMARY KEY,
    CompanyID CHAR(36) NOT NULL,
    RoleID CHAR(36) NOT NULL,
    PermissionGroupID CHAR(36) NOT NULL,
    AssignedByUserID CHAR(36) NOT NULL,
    AssignedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Notes TEXT,

    INDEX idx_CURPGA_CompanyID (CompanyID),
    INDEX idx_CURPGA_RoleID (RoleID),
    INDEX idx_CURPGA_PermissionGroupID (PermissionGroupID),
    INDEX idx_CURPGA_AssignedByUserID (AssignedByUserID)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;