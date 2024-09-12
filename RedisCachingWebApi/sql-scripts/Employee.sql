-- Create the Employee database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Employee')
BEGIN
    CREATE DATABASE Employee;
END
GO
USE [Employee];
GO
IF OBJECT_ID('Employee', 'U') IS NOT NULL
BEGIN
    DROP TABLE Employee;
END
GO
CREATE TABLE Employee (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),
    EmpName NVARCHAR(20) NOT NULL,
    EmpDesignation NVARCHAR(20) NULL,
    ProjectName NVARCHAR(20) NULL,
    Skill NVARCHAR(20) NULL,
    salary decimal(12,4) null,
    Test int null,
    test2 decimal(10,2) null,
    test34 int null
);
GO
