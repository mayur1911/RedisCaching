CREATE PROCEDURE sp_DeleteManagerById  
 @ManagerId AS INT  
AS  
BEGIN  
 DELETE FROM Manager  where ManagerID = @ManagerId
END
GO