ALTER TABLE CompanySessionAudit
ADD CONSTRAINT fk_CompanySessionAudit_CompanyID FOREIGN KEY (CompanyID) REFERENCES CompanyInfo(CompanyID);

ALTER TABLE CompanySessionAudit
ADD CONSTRAINT fk_CompanySessionAudit_SessionID FOREIGN KEY (SessionID) REFERENCES CompanySessions(SessionID);

ALTER TABLE CompanySessionAudit
ADD CONSTRAINT fk_CompanySessionAudit_UserID FOREIGN KEY (UserID) REFERENCES CompanyUsers(UserID);