CREATE PROCEDURE [dbo].[User_GetUserName]
	@userId int
AS
	SELECT username FROM Users WHERE userId=@userId