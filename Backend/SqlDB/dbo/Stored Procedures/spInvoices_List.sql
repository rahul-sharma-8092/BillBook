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

    CREATE TABLE #Filtered (
        Id INT,
        InvoiceNo NVARCHAR(50),
        CustomerId INT,
        CustomerName NVARCHAR(200),
        Mobile NVARCHAR(20),
        SubTotal DECIMAL(10,2),
        DiscountType NVARCHAR(10),
        DiscountValue DECIMAL(10, 2),
        DiscountAmount DECIMAL(10,2),
        TotalAmount DECIMAL(10,2),
        PaidAmount DECIMAL(10,2),
        BalanceAmount DECIMAL(10,2),
        PaymentMode NVARCHAR(50),
        Notes NVARCHAR(500),
        CreatedAt DATETIME
    );

    INSERT INTO #Filtered
    SELECT
        i.[Id], i.[InvoiceNo], i.[CustomerId], i.[CustomerName], i.[Mobile],
        i.[SubTotal], i.[DiscountType], i.[DiscountValue], i.[DiscountAmount], i.[TotalAmount], i.[PaidAmount], i.[BalanceAmount],
        i.[PaymentMode], i.[Notes], i.[CreatedAt]
    FROM [dbo].[Invoices] i
    WHERE
        (@InvoiceNo IS NULL OR i.[InvoiceNo] LIKE '%' + @InvoiceNo + '%')
        AND (@Customer IS NULL OR i.[CustomerName] LIKE '%' + @Customer + '%')
        AND (@Mobile IS NULL OR i.[Mobile] LIKE '%' + @Mobile + '%');

    SELECT COUNT(1) AS [TotalCount] FROM #Filtered;

    SELECT *
    FROM #Filtered i
    ORDER BY [CreatedAt] DESC
    OFFSET (@Page - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    DROP TABLE #Filtered;
END