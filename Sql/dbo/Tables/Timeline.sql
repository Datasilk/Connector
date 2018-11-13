CREATE TABLE [dbo].[Timeline]
(
	[timelineId] CHAR(32) NOT NULL PRIMARY KEY, /* uses a GUID */
    [userId] INT NOT NULL, 
    [subscriptionId] INT NULL, /* Id required only if this timeline event was obtained from a subscription-based timeline */
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE(), 
	[read] INT NOT NULL DEFAULT 0,
    [message] NVARCHAR(MAX) NULL, 
    [photo] INT NULL, 
    [gallery] INT NULL 
)

GO

CREATE INDEX [IX_Timeline_DateCreated] ON [dbo].[Timeline] ([userId], [datecreated] DESC)
