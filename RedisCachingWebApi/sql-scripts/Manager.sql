-- Create the Employee database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Employee')
BEGIN
    CREATE DATABASE Employee;
END
GO
USE [Employee];
GO
IF OBJECT_ID('Manager', 'U') IS NOT NULL
BEGIN
    DROP TABLE Manager;
END
GO
CREATE TABLE Manager (
    ManagerID INT PRIMARY KEY IDENTITY(1,1),
    ManagerName NVARCHAR(20) NOT NULL,
    ManagerDesignation NVARCHAR(20) NULL,
    ProjectName NVARCHAR(20) NULL,
    Skill NVARCHAR(20) NULL,
    Education NVARCHAR(20) NULL, -- New column added here
    salary decimal(7,4) null,
    Test int null,
    test34 int null
);
GO
