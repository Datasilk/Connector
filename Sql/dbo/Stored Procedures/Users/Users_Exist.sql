﻿CREATE PROCEDURE [dbo].[Users_Exist]
AS
	IF(SELECT COUNT(*) FROM Users) > 0 BEGIN
		SELECT 1
	END ELSE BEGIN
		SELECT 0
	END
