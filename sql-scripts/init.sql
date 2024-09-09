-- Create the Employee database
CREATE DATABASE Employee;

-- Switch to the Employee database
USE Employee;

-- Create the Employee table
CREATE TABLE Employee (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),
    EmpName NVARCHAR(20) NOT NULL,
    EmpDesignation NVARCHAR(20) NULL,
    ProjectName NVARCHAR(20) NULL,
    Skill NVARCHAR(20) NULL
);