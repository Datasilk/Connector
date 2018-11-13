CREATE PROCEDURE [dbo].[Subscriber_GetUserName]
	@subscriberId int
AS
	SELECT username FROM Subscribers WHERE subscriberId=@subscriberId