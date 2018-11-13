CREATE TABLE [dbo].[HashTags]
(
	[hashtagId] INT NOT NULL PRIMARY KEY, 
    [hashtag] NVARCHAR(64) NOT NULL, 
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE()
)

GO

CREATE INDEX [IX_HashTags_Hashtag] ON [dbo].[HashTags] ([hashtag])
