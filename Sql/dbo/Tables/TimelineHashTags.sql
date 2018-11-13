CREATE TABLE [dbo].[TimelineHashTags]
(
    [hashtagId] INT NOT NULL PRIMARY KEY, 
	[timelineId] CHAR(32) NOT NULL 
)

GO

CREATE INDEX [IX_TimelineHashTags_TimelineId] ON [dbo].[TimelineHashTags] ([timelineId])
