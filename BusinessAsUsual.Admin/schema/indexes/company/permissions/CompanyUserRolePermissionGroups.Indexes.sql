CREATE INDEX idx_CURPG_CompanyID ON CompanyUserRolePermissionGroups (CompanyID);
CREATE INDEX idx_CURPG_GroupName ON CompanyUserRolePermissionGroups (GroupName);
CREATE INDEX idx_CURPG_IsActive ON CompanyUserRolePermissionGroups (IsActive);
CREATE INDEX idx_CURPG_CreatedByUserID ON CompanyUserRolePermissionGroups (CreatedByUserID);