CREATE TABLE [dbo].[SubscriberRoles]
(
	[subscriberId] INT NOT NULL PRIMARY KEY, 
    [roleId] INT NOT NULL, 
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE()
)

GO

CREATE INDEX [IX_SubscriberRoles_RoleId] ON [dbo].[SubscriberRoles] ([roleId])
