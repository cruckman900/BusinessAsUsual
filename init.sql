DROP DATABASE IF EXISTS [TestDb];
GO

CREATE DATABASE [TestDb];
GO

EXEC sp_configure 'remote access', 1;
RECONFIGURE;

USE [TestDb];
GO

CREATE TABLE TestPing (Id INT);
GO

USE [TestDb];
GO

SELECT name FROM sys.tables;
GO