/* 
	A list of users who have requested a subscription to node services
*/
CREATE TABLE [dbo].[Subscribers]
(
	[subscriberId] INT NOT NULL PRIMARY KEY, 
    [userId] INT NOT NULL , 
    [active] BIT NOT NULL DEFAULT 1, 
    [username] NVARCHAR(64) NOT NULL, 
    [key] NVARCHAR(64) NOT NULL DEFAULT '' , 

    [datecreated] DATETIME NOT NULL DEFAULT GETDATE()
)

GO

CREATE INDEX [IX_Subscribers_UserId] ON [dbo].[Subscribers] ([userId])

GO

CREATE INDEX [IX_Subscribers_UserName] ON [dbo].[Subscribers] ([username])

GO

CREATE INDEX [IX_Subscribers_Key] ON [dbo].[Subscribers] ([key])

GO

CREATE INDEX [IX_Subscribers_DateCreated] ON [dbo].[Subscribers] ([datecreated] DESC)
