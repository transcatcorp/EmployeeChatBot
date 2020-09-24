CREATE PROCEDURE[dbo].[Report_CheckByEmployeeId]
	@EmployeeId nvarchar(MAX)
AS
BEGIN
	SET NOCOUNT ON;
SET XACT_ABORT ON;
	
	SELECT TOP(1) *
	FROM dbo.Report
	WHERE EmployeeId = @EmployeeId
	ORDER BY CreatedAt DESC
END

