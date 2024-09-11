-- Create the Employee database if it does not exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Employee')
BEGIN
    CREATE DATABASE Employee;
END
GO

-- Switch to the Employee database
USE Employee;
GO

-- Create the Employee table if it does not exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Employee]') AND type in (N'U'))
BEGIN
    CREATE TABLE Employee (
        EmployeeID INT PRIMARY KEY IDENTITY(1,1),
        EmpName NVARCHAR(20) NOT NULL,
        EmpDesignation NVARCHAR(20) NULL,
        ProjectName NVARCHAR(20) NULL,
        Skill NVARCHAR(20) NULL
    );
END
GO