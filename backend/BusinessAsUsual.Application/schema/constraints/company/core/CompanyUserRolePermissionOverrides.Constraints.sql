ALTER TABLE CompanyUserRolePermissionOverrides
ADD CONSTRAINT fk_CURPO_UserID FOREIGN KEY (UserID) REFERENCES CompanyUsers(UserID);

ALTER TABLE CompanyUserRolePermissionOverrides
ADD CONSTRAINT fk_CURPO_PermissionID FOREIGN KEY (PermissionID) REFERENCES CompanyUserRolePermissions(PermissionID);