CREATE TABLE CompanySessionSettings (
    SessionSettingID CHAR(36) NOT NULL PRIMARY KEY,
    CompanyID CHAR(36) NOT NULL,
    MaxSessionDurationMinutes INT UNSIGNED DEFAULT 1440, -- 24 hours
    AllowMultipleSessions BOOLEAN DEFAULT FALSE,
    RequireIPMatch BOOLEAN DEFAULT TRUE,
    RequireUserAgentMatch BOOLEAN DEFAULT TRUE,
    AutoTerminateOnInactivity BOOLEAN DEFAULT TRUE,
    InactivityTimeoutMinutes INT UNSIGNED DEFAULT 30,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;