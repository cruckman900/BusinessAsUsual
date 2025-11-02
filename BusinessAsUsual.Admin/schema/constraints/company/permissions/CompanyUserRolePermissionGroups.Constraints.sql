ALTER TABLE CompanyUserRolePermissionGroups
ADD CONSTRAINT fk_CURPG_CompanyID FOREIGN KEY (CompanyID) REFERENCES CompanyInfo(CompanyID);

ALTER TABLE CompanyUserRolePermissionGroups
ADD CONSTRAINT fk_CURPG_CreatedByUserID FOREIGN KEY (CreatedByUserID) REFERENCES CompanyUsers(UserID);