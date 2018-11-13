CREATE TABLE [dbo].[SubscriptionsQueried]
(
	[queriedId] INT NOT NULL PRIMARY KEY, 
    [subscriptionId] INT NOT NULL, 
    [userId] INT NOT NULL, 
    [utc] INT NOT NULL DEFAULT 7, 
    [datecreated] DATETIME NOT NULL DEFAULT GETUTCDATE() 
)

GO

CREATE INDEX [IX_SubscriptionsQueried_DateCreated] ON [dbo].[SubscriptionsQueried] ([datecreated] DESC)
