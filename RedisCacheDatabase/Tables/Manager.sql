CREATE TABLE Manager (
    ManagerID INT PRIMARY KEY IDENTITY(1,1),
    ManagerName NVARCHAR(20) NOT NULL,
    ManagerDesignation NVARCHAR(20) NULL,
    ProjectName NVARCHAR(20) NULL,
    Skill NVARCHAR(20) NULL,
    Education NVARCHAR(20) NULL, -- New column added here
    salary decimal(7,4) null,
    Test int null,
    test34 int null
);
GO