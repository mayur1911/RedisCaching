-- Create the Employee database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Employee')
BEGIN
    CREATE DATABASE Employee;
END
GO
USE [Employee];
GO
CREATE TABLE Manager (
    ManagerID INT PRIMARY KEY IDENTITY(1,1),
    ManagerName NVARCHAR(20) NOT NULL,
    ManagerDesignation NVARCHAR(20) NULL,
    ProjectName NVARCHAR(20) NULL
);
GO
