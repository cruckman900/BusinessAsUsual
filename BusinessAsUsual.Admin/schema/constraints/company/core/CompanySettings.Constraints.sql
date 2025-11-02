ALTER TABLE CompanySettings
ADD CONSTRAINT fk_CompanySettings_CompanyID FOREIGN KEY (CompanyID) REFERENCES CompanyInfo(CompanyID);