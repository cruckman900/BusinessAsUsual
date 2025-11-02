ALTER TABLE CompanyUserRolePermissionGroupOverrides
ADD CONSTRAINT fk_CURPGO_CompanyID FOREIGN KEY (CompanyID) REFERENCES CompanyInfo(CompanyID);

ALTER TABLE CompanyUserRolePermissionGroupOverrides
ADD CONSTRAINT fk_CURPGO_PermissionGroupID FOREIGN KEY (PermissionGroupID) REFERENCES CompanyUserRolePermissionGroups(PermissionGroupID);

ALTER TABLE CompanyUserRolePermissionGroupOverrides
ADD CONSTRAINT fk_CURPGO_RoleID FOREIGN KEY (RoleID) REFERENCES CompanyUserRoles(RoleID);

ALTER TABLE CompanyUserRolePermissionGroupOverrides
ADD CONSTRAINT fk_CURPGO_PermissionID FOREIGN KEY (PermissionID) REFERENCES CompanyUserRolePermissions(PermissionID);

ALTER TABLE CompanyUserRolePermissionGroupOverrides
ADD CONSTRAINT fk_CURPGO_OverriddenByUserID FOREIGN KEY (OverriddenByUserID) REFERENCES CompanyUsers(UserID);