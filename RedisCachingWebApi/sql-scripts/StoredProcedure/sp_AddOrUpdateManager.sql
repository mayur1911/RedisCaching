-- Create the Employee database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Employee')
BEGIN
    CREATE DATABASE Employee;
END
GO
USE [Employee];
GO
CREATE PROCEDURE sp_AddOrUpdateManager
    @ManagerID INT = NULL,           -- ManagerID (NULL for new manager, or the ID for update)
    @ManagerName NVARCHAR(50),       -- Manager Name
    @ManagerDesignation NVARCHAR(50) = NULL, -- Manager Designation (optional)
    @ProjectName NVARCHAR(50) = NULL,        -- Project Name (optional)
    @newId INT OUTPUT                -- Output parameter to return the ManagerID
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if the record exists for the given @ManagerID
    IF EXISTS (SELECT 1 FROM Manager WHERE ManagerID = @ManagerID)
    BEGIN
        -- Update the existing manager's details
        UPDATE Manager
        SET ManagerName = @ManagerName,
            ManagerDesignation = @ManagerDesignation,
            ProjectName = @ProjectName
        WHERE ManagerID = @ManagerID;

        -- Return the existing ManagerID
        SET @newId = @ManagerID;
    END
    ELSE
    BEGIN
        -- Insert new manager and set @newId to the new ManagerID
        INSERT INTO Manager (ManagerName, ManagerDesignation, ProjectName)
        VALUES (@ManagerName, @ManagerDesignation, @ProjectName);

        -- Return the newly inserted ManagerID
        SET @newId = SCOPE_IDENTITY();
    END
END
GO
