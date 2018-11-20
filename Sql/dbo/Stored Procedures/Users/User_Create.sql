CREATE PROCEDURE [dbo].[User_Create]
	@name nvarchar(64),
	@email nvarchar(64),
	@username nvarchar(64),
	@password nvarchar(255),
	@photo bit = 0,
	@status smallint = 1
AS
	DECLARE @id int = NEXT VALUE FOR SequenceUsers
	INSERT INTO Users (userId, [name], email, username, [password], photo, [status])
	VALUES (@id, @name, @email, @username, @password, @photo, @status)
	
	SELECT @id