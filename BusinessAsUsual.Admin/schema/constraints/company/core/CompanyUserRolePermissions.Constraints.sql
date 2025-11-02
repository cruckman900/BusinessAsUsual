ALTER TABLE CompanyUserRolePermissions
ADD CONSTRAINT fk_CURP_RoleID FOREIGN KEY (RoleID) REFERENCES CompanyUserRoles(RoleID);