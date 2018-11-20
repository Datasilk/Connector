CREATE TABLE [dbo].[Comments]
(
	[commentId] CHAR(32) NOT NULL PRIMARY KEY, 
    [timelineId] CHAR(32) NOT NULL, 
    [username] NVARCHAR(32) NOT NULL, 
    [datecreated] DATETIME NOT NULL,
    [message] NVARCHAR(MAX) NOT NULL
)
