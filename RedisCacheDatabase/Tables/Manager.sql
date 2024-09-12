CREATE TABLE Manager (
    ManagerID INT PRIMARY KEY IDENTITY(1,1),
    ManagerName NVARCHAR(20) NOT NULL,
    ManagerDesignation NVARCHAR(20) NULL,
    ProjectName NVARCHAR(20) NULL
);
GO