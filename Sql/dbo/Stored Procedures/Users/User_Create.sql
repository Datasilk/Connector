CREATE PROCEDURE [dbo].[User_Create]
	@username nvarchar(64),
	@key nvarchar(64),
	@password nvarchar(255)
AS
	DECLARE @id int = NEXT VALUE FOR SequenceUsers
	INSERT INTO Users (userId, username, [password], datecreated)
	VALUES (@id, @username, @key, @password, GETDATE())
	
	SELECT @id