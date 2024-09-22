-- Create the Employee database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Employee')
BEGIN
    CREATE DATABASE Employee;
END
GO
USE [Employee];
GO
CREATE TABLE Employee (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),
    ManagerId INT NOT NULL,  -- Foreign key referencing ManagerID in Manager table
    EmpName NVARCHAR(20) NOT NULL,
    EmpDesignation NVARCHAR(20) NULL,
    ProjectName NVARCHAR(20) NULL,
    Skill NVARCHAR(20) NULL,
    CONSTRAINT FK_Employee_Manager FOREIGN KEY (ManagerId)
    REFERENCES Manager(ManagerID)  -- Establish the foreign key constraint
    ON DELETE CASCADE ON UPDATE CASCADE  -- Optional: Cascade delete and update
);
GO
