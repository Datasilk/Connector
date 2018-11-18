CREATE PROCEDURE [dbo].[Subscriber_GetKey]
	@userId int,
	@username nvarchar(100)
AS
	SELECT [key] FROM Subscribers WHERE userId=@userId AND username=@username