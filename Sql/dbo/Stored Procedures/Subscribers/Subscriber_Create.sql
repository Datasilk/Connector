CREATE PROCEDURE [dbo].[Subscriber_Create]
	@username nvarchar(64),
	@key nvarchar(64),
	@password nvarchar(255)
AS
	DECLARE @id int = NEXT VALUE FOR SequenceSubscribers
	INSERT INTO Subscribers (subscriberId, username, [key], [password], datecreated)
	VALUES (@id, @username, @key, @password, GETDATE())
	
	SELECT @id