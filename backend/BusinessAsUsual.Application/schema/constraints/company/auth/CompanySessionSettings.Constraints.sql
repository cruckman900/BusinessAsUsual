ALTER TABLE CompanySessionSettings
ADD CONSTRAINT fk_CompanySessionSettings_CompanyID FOREIGN KEY (CompanyID) REFERENCES CompanyInfo(CompanyID);