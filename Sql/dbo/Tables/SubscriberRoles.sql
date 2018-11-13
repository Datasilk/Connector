CREATE TABLE [dbo].[SubscriberRoles]
(
	[subscriberId] INT NOT NULL PRIMARY KEY, 
    [roleId] INT NOT NULL, 
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE()
)
