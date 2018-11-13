CREATE TABLE [dbo].[HashTagRoles]
(
	[hashtagId] INT NOT NULL PRIMARY KEY, 
    [roleId] INT NOT NULL, 
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE() 
)

GO

CREATE INDEX [IX_HashTagRoles_RoleId] ON [dbo].[HashTagRoles] ([roleId])
