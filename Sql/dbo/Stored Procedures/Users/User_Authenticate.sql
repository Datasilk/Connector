﻿CREATE PROCEDURE [dbo].[User_Authenticate] 
	@username nvarchar(64) = '',
	@password nvarchar(255) = ''
AS
BEGIN
	SELECT *
	FROM Users
	WHERE username=@username AND [password]=@password
END