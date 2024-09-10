-- Create the Employee database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Employee')
BEGIN
    PRINT 'Creating Employee database...';
    CREATE DATABASE Employee;
END
GO

-- Switch to the Employee database
USE Employee;
GO

-- Create the Employee table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Employee')
BEGIN
    PRINT 'Creating Employee table...';
    CREATE TABLE Employee (
        EmployeeID INT PRIMARY KEY IDENTITY(1,1),
        EmpName NVARCHAR(20) NOT NULL,
        EmpDesignation NVARCHAR(20) NULL,
        ProjectName NVARCHAR(20) NULL,
        Skill NVARCHAR(20) NULL
    );
END
GO