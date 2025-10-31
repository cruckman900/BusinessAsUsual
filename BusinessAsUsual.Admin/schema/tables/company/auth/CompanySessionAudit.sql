CREATE TABLE CompanySessionAudit (
    AuditID CHAR(36) NOT NULL PRIMARY KEY,
    CompanyID CHAR(36) NOT NULL,
    SessionID CHAR(36) NOT NULL,
    UserID CHAR(36) NOT NULL,
    ActionType VARCHAR(100) NOT NULL,                         -- e.g., "Login", "Logout", "TokenRefresh"
    ActionTimestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    IPAddress VARCHAR(45),
    UserAgent TEXT,
    Notes TEXT,

    INDEX idx_CompanySessionAudit_CompanyID (CompanyID),
    INDEX idx_CompanySessionAudit_SessionID (SessionID),
    INDEX idx_CompanySessionAudit_UserID (UserID)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;