CREATE PROCEDURE [dbo].[User_GetPassword]
	@username nvarchar(32)
AS
	SELECT [password] FROM Users WHERE username=@username