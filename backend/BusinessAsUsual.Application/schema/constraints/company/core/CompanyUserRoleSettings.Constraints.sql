ALTER TABLE CompanyUserRoleSettings
ADD CONSTRAINT fk_CompanyUserRoleSettings_RoleID FOREIGN KEY (RoleID) REFERENCES CompanyUserRoles(RoleID);