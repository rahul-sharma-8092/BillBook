CREATE PROCEDURE [dbo].[spDashboard_MonthlyTrend]
    @Months INT = 12
AS
BEGIN
    SET NOCOUNT ON;

    IF (@Months IS NULL OR @Months < 1) SET @Months = 12;
    IF (@Months > 36) SET @Months = 36;

    ;WITH MonthsCTE AS (
        SELECT DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) AS [FirstOfMonth], 1 AS [N]
        UNION ALL
        SELECT DATEADD(MONTH, -1, [FirstOfMonth]), [N] + 1
        FROM MonthsCTE
        WHERE [N] < @Months
    )
    SELECT
        CONVERT(NVARCHAR(7), m.[FirstOfMonth], 120) AS [YearMonth],
        CAST(
            ISNULL(
                SUM(i.[TotalAmount]),
                0
            )
        AS DECIMAL(10,2)) AS [Total]
    FROM MonthsCTE m
    LEFT JOIN [dbo].[Invoices] i
        ON i.[CreatedAt] >= m.[FirstOfMonth]
        AND i.[CreatedAt] < DATEADD(MONTH, 1, m.[FirstOfMonth])
    GROUP BY m.[FirstOfMonth]
    ORDER BY m.[FirstOfMonth];
END

