CREATE PROCEDURE [dbo].[spDashboard_DailySales]
    @Days INT = 14
AS
BEGIN
    SET NOCOUNT ON;

    IF (@Days IS NULL OR @Days < 1) SET @Days = 14;
    IF (@Days > 90) SET @Days = 90;

    ;WITH DaysCTE AS (
        SELECT CAST(DATEADD(DAY, -(@Days - 1), CAST(GETDATE() AS DATE)) AS DATE) AS [Dt]
        UNION ALL
        SELECT DATEADD(DAY, 1, [Dt])
        FROM DaysCTE
        WHERE [Dt] < CAST(GETDATE() AS DATE)
    )
    SELECT
        d.[Dt] AS [Date],
        CAST(ISNULL(SUM(i.[TotalAmount]), 0) AS DECIMAL(10,2)) AS [Total]
    FROM DaysCTE d
    LEFT JOIN [dbo].[Invoices] i
        ON CAST(i.[CreatedAt] AS DATE) = d.[Dt]
    GROUP BY d.[Dt]
    ORDER BY d.[Dt]
    OPTION (MAXRECURSION 1000);
END

