﻿CREATE TABLE [dbo].[Subscriptions]
(
	[subscriptionId] INT NOT NULL PRIMARY KEY, 
    [userId] INT NOT NULL, 
    [domain] NVARCHAR(64) NOT NULL, 
    [username] NVARCHAR(32) NOT NULL, 
    [key] NVARCHAR(64) NOT NULL, 
    [title] NVARCHAR(64) NOT NULL, 
    [favorite] BIT NOT NULL DEFAULT 0, 
    [categoryId] INT NULL , 
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE()
)
