CREATE PROCEDURE [dbo].[Subscriber_GetDetails]
	@subscriberId int
AS
	SELECT * FROM Subscribers WHERE subscriberId=@subscriberId