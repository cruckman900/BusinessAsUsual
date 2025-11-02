ALTER TABLE CompanyUserRoles
ADD CONSTRAINT fk_CompanyUserRoles_CompanyID FOREIGN KEY (CompanyID) REFERENCES CompanyInfo(CompanyID);