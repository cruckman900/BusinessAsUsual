DROP DATABASE IF EXISTS [BusinessAsUsual];
GO

CREATE DATABASE [BusinessAsUsual];
GO

EXEC sp_configure 'remote access', 1;
RECONFIGURE;

USE [BusinessAsUsual];
GO

CREATE TABLE TestPing (Id INT);
GO

USE [BusinessAsUsual];
GO

SELECT name FROM sys.tables;
GO