ALTER TABLE CompanyUserRolePermissionGroupAssignments
ADD CONSTRAINT fk_CURPGA_CompanyID FOREIGN KEY (CompanyID) REFERENCES CompanyInfo(CompanyID);

ALTER TABLE CompanyUserRolePermissionGroupAssignments
ADD CONSTRAINT fk_CURPGA_RoleID FOREIGN KEY (RoleID) REFERENCES CompanyUserRoles(RoleID);

ALTER TABLE CompanyUserRolePermissionGroupAssignments
ADD CONSTRAINT fk_CURPGA_PermissionGroupID FOREIGN KEY (PermissionGroupID) REFERENCES CompanyUserRolePermissionGroups(PermissionGroupID);

ALTER TABLE CompanyUserRolePermissionGroupAssignments
ADD CONSTRAINT fk_CURPGA_AssignedByUserID FOREIGN KEY (AssignedByUserID) REFERENCES CompanyUsers(UserID);