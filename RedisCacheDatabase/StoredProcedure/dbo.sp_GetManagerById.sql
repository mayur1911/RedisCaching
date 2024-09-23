CREATE PROCEDURE sp_GetManagerById
 @ManagerId AS INT
AS
BEGIN
    SELECT ManagerID, ManagerName, ManagerDesignation, ProjectName
    FROM Manager WITH (NOLOCK)
    WHERE ManagerID = @ManagerID;
END
GO