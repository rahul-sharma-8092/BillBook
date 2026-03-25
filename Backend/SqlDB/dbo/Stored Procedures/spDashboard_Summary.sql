CREATE PROCEDURE [dbo].[spDashboard_Summary]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATE = CAST(GETDATE() AS DATE);

    SELECT
        CAST(ISNULL(SUM(CASE WHEN CAST([CreatedAt] AS DATE) = @Today THEN [TotalAmount] ELSE 0 END), 0) AS DECIMAL(10,2)) AS [TodaySales],
        COUNT(1) AS [TotalInvoices]
    FROM [dbo].[Invoices];
END

