/* 
	A log of queries executed via the REST API
*/
CREATE TABLE [dbo].[Queries]
(
	[queryId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE(), 
    [userId] INT NULL, 
    [query] INT NOT NULL, 
    [args] NVARCHAR(64) NULL, 
    [ipaddress] NVARCHAR(26) NOT NULL DEFAULT '', 
    [username] NVARCHAR(32) NULL
)

GO

CREATE INDEX [IX_Queries_DateCreated] ON [dbo].[Queries] ([datecreated] DESC)

GO

CREATE INDEX [IX_Queries_UserId] ON [dbo].[Queries] ([userId])
