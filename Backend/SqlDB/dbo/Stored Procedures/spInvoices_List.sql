CREATE PROCEDURE [dbo].[spInvoices_List]
    @InvoiceNo NVARCHAR(50) = NULL,
    @Customer NVARCHAR(200) = NULL,
    @Mobile NVARCHAR(20) = NULL,
    @Page INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    IF (@Page IS NULL OR @Page < 1) SET @Page = 1;
    IF (@PageSize IS NULL OR @PageSize < 1) SET @PageSize = 20;
    IF (@PageSize > 200) SET @PageSize = 200;

    ;WITH Filtered AS (
        SELECT
            i.[Id], i.[InvoiceNo], i.[CustomerId], i.[CustomerName], i.[Mobile],
            i.[SubTotal], i.[DiscountAmount], i.[TotalAmount], i.[PaidAmount], i.[BalanceAmount],
            i.[PaymentMode], i.[Notes], i.[CreatedAt]
        FROM [dbo].[Invoices] i
        WHERE
            (@InvoiceNo IS NULL OR i.[InvoiceNo] LIKE '%' + @InvoiceNo + '%')
            AND (@Customer IS NULL OR i.[CustomerName] LIKE '%' + @Customer + '%')
            AND (@Mobile IS NULL OR i.[Mobile] LIKE '%' + @Mobile + '%')
    )
    SELECT COUNT(1) AS [TotalCount] FROM Filtered;

    SELECT *
    FROM Filtered
    ORDER BY [CreatedAt] DESC
    OFFSET (@Page - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END

