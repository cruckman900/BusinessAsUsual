ALTER TABLE CompanyUserRolePermissionGroupMembers
ADD CONSTRAINT fk_CURPGM_CompanyID FOREIGN KEY (CompanyID) REFERENCES CompanyInfo(CompanyID);

ALTER TABLE CompanyUserRolePermissionGroupMembers
ADD CONSTRAINT fk_CURPGM_PermissionGroupID FOREIGN KEY (PermissionGroupID) REFERENCES CompanyUserRolePermissionGroups(PermissionGroupID);

ALTER TABLE CompanyUserRolePermissionGroupMembers
ADD CONSTRAINT fk_CURPGM_PermissionID FOREIGN KEY (PermissionID) REFERENCES CompanyUserRolePermissions(PermissionID);

ALTER TABLE CompanyUserRolePermissionGroupMembers
ADD CONSTRAINT fk_CURPGM_AddedByUserID FOREIGN KEY (AddedByUserID) REFERENCES CompanyUsers(UserID);