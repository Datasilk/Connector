CREATE PROCEDURE [dbo].[Subscriber_Authenticate] 
	@username nvarchar(64) = '',
	@password nvarchar(255) = ''
AS
BEGIN
	SELECT *
	FROM Subscribers
	WHERE username=@username AND [password]=@password
END