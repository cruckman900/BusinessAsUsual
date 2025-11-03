ALTER TABLE CompanyUsers
ADD CONSTRAINT fk_CompanyUsers_CompanyID FOREIGN KEY (CompanyID) REFERENCES CompanyInfo(CompanyID);