CREATE PROCEDURE sp_AddOrUpdateManager  
    @ManagerID INT = NULL,           -- ManagerID (NULL for new manager, or the ID for update)  
    @ManagerName NVARCHAR(50),       -- Manager Name  
    @ManagerDesignation NVARCHAR(50) = NULL, -- Manager Designation (optional)  
    @ProjectName NVARCHAR(50) = NULL,        -- Project Name (optional)  
    @newId INT=0 OUTPUT              -- Output parameter to return the ManagerID  
AS  
BEGIN  
    -- Disable the message indicating how many rows were affected by a statement
    SET NOCOUNT OFF;  

    -- Try to select the ManagerID from the Manager table
    SELECT @newId = ManagerID 
    FROM Manager WITH (NOLOCK)
    WHERE ManagerID = @ManagerID;

    -- Check if the record exists based on the selected @newId value
    IF @newId > 0
    BEGIN  
        -- Update the existing manager's details  
        UPDATE Manager  
        SET ManagerName = @ManagerName,  
            ManagerDesignation = @ManagerDesignation,  
            ProjectName = @ProjectName  
        WHERE ManagerID = @ManagerID;
    END  
    ELSE  
    BEGIN  
        -- Insert a new manager and set @newId to the new ManagerID  
        INSERT INTO Manager (ManagerName, ManagerDesignation, ProjectName)  
        VALUES (@ManagerName, @ManagerDesignation, @ProjectName);  
  
        -- Return the newly inserted ManagerID  
        SET @newId = SCOPE_IDENTITY();  
    END  
END  
GO