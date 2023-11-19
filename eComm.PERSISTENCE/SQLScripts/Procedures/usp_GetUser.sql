CREATE PROCEDURE usp_GetUser
	@Username VARCHAR(55)
AS
	SELECT * FROM Users WHERE Username = @Username;
GO