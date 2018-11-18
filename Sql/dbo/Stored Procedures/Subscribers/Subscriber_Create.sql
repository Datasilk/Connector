CREATE PROCEDURE [dbo].[Subscriber_Create]
	@userId int,
	@username nvarchar(64),
	@key nvarchar(64)
AS
	DECLARE @id int = NEXT VALUE FOR SequenceSubscribers
	INSERT INTO Subscribers (subscriberId, userId, username, [key], datecreated)
	VALUES (@id, @userId, @username, @key, GETDATE())
	
	SELECT @id