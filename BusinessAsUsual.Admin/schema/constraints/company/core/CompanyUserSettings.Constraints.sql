ALTER TABLE CompanyUserSettings
ADD CONSTRAINT fk_CompanyUserSettings_UserID FOREIGN KEY (UserID) REFERENCES CompanyUsers(UserID);