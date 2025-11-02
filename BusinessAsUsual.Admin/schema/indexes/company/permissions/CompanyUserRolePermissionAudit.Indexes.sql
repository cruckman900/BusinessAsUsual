CREATE INDEX idx_CURPA_CompanyID ON CompanyUserRolePermissionAudit (CompanyID);
CREATE INDEX idx_CURPA_RoleID ON CompanyUserRolePermissionAudit (RoleID);
CREATE INDEX idx_CURPA_PermissionID ON CompanyUserRolePermissionAudit (PermissionID);
CREATE INDEX idx_CURPA_ChangedByUserID ON CompanyUserRolePermissionAudit (ChangedByUserID);