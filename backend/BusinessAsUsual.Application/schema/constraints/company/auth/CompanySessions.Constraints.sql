ALTER TABLE CompanySessions
ADD CONSTRAINT fk_CompanySessions_CompanyID FOREIGN KEY (CompanyID) REFERENCES CompanyInfo(CompanyID);

ALTER TABLE CompanySessions
ADD CONSTRAINT fk_CompanySessions_UserID FOREIGN KEY (UserID) REFERENCES CompanyUsers(UserID);