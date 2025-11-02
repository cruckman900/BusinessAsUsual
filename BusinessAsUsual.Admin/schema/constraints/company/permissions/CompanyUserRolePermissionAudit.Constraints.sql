ALTER TABLE CompanyUserRolePermissionAudit
ADD CONSTRAINT fk_CURPA_CompanyID FOREIGN KEY (CompanyID) REFERENCES CompanyInfo(CompanyID);

ALTER TABLE CompanyUserRolePermissionAudit
ADD CONSTRAINT fk_CURPA_RoleID FOREIGN KEY (RoleID) REFERENCES CompanyUserRoles(RoleID);

ALTER TABLE CompanyUserRolePermissionAudit
ADD CONSTRAINT fk_CURPA_PermissionID FOREIGN KEY (PermissionID) REFERENCES CompanyUserRolePermissions(PermissionID);

ALTER TABLE CompanyUserRolePermissionAudit
ADD CONSTRAINT fk_CURPA_ChangedByUserID FOREIGN KEY (ChangedByUserID) REFERENCES CompanyUsers(UserID);