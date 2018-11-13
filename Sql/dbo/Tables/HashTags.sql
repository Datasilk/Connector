CREATE TABLE [dbo].[HashTags]
(
	[hashtagId] INT NOT NULL PRIMARY KEY, 
    [hashtag] NVARCHAR(64) NOT NULL, 
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE()
)
