CREATE TABLE [dbo].[TimelineHashTags]
(
    [hashtagId] INT NOT NULL PRIMARY KEY, 
	[timelineId] CHAR(32) NOT NULL, 
    CONSTRAINT [AK_TimelineHashTags_TimelineId] UNIQUE ([timelineId])
)
