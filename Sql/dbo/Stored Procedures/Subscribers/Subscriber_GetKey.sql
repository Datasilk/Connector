CREATE PROCEDURE [dbo].[Subscriber_GetKey]
	@username nvarchar(100)
AS
	SELECT [password] FROM Subscribers WHERE username=@username