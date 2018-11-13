CREATE TABLE [dbo].[Roles]
(
	[roleId] INT NOT NULL PRIMARY KEY, 
    [name] NVARCHAR(32) NOT NULL, 
    [security] BINARY(512) NOT NULL
)

GO

CREATE INDEX [IX_Roles_Name] ON [dbo].[Roles] ([name])
