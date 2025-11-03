ALTER TABLE CompanyUserRolePermissionGroupAudit
ADD CONSTRAINT fk_CURPGA_CompanyID FOREIGN KEY (CompanyID) REFERENCES CompanyInfo(CompanyID);

ALTER TABLE CompanyUserRolePermissionGroupAudit
ADD CONSTRAINT fk_CURPGA_PermissionGroupID FOREIGN KEY (PermissionGroupID) REFERENCES CompanyUserRolePermissionGroups(PermissionGroupID);

ALTER TABLE CompanyUserRolePermissionGroupAudit
ADD CONSTRAINT fk_CURPGA_ChangedByUserID FOREIGN KEY (ChangedByUserID) REFERENCES CompanyUsers(UserID);