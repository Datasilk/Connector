CREATE PROCEDURE [dbo].[Subscriber_UpdatePassword]
	@subscriberId int,
	@password nvarchar(255)
AS
	UPDATE Subscribers SET [password]=@password WHERE subscriberId=@subscriberId