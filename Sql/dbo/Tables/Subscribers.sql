/* 
	A list of users who have requested a subscription to node services
*/
CREATE TABLE [dbo].[Subscribers]
(
	[subscriberId] INT NOT NULL PRIMARY KEY, 
    [active] BIT NOT NULL DEFAULT 1, 
    [username] NVARCHAR(64) NOT NULL, 
    [key] NVARCHAR(64) NOT NULL DEFAULT '' , 
    [password] NVARCHAR(255) NOT NULL DEFAULT '', 

    [datecreated] DATETIME NOT NULL DEFAULT GETDATE() 
)
