CREATE TABLE [dbo].[Users]
(
	[userId] INT NOT NULL PRIMARY KEY, 
    [name] NVARCHAR(64) NOT NULL DEFAULT '', 
    [email] NVARCHAR(64) NOT NULL DEFAULT '', 
    [username] NVARCHAR(32) NOT NULL, 
    [password] NVARCHAR(255) NOT NULL, 
    [photo] BIT NOT NULL DEFAULT 0, 
    [status] SMALLINT NOT NULL DEFAULT 1, 
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE(), 
    [lastlogin] DATETIME NOT NULL DEFAULT GETDATE()
)

GO

CREATE INDEX [IX_Users_UserName] ON [dbo].[Users] ([username])
