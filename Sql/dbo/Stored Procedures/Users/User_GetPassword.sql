CREATE PROCEDURE [dbo].[User_GetKey]
	@username nvarchar(100)
AS
	SELECT [password] FROM Users WHERE username=@username