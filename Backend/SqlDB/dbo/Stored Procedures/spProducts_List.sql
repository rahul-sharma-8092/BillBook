CREATE PROCEDURE [dbo].[spProducts_List]
    @Q NVARCHAR(200) = NULL,
    @CategoryId INT = NULL,
    @OnlyActive BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.[Id], p.[CategoryId], p.[ProductType], p.[Name], p.[ModelNo], p.[SerialNo],
        p.[WarrantyMonths], p.[WarrantyNote], p.[InvoiceNote],
        p.[PurchasePrice], p.[SellPrice], p.[StockQty],
        p.[DiscountType], p.[DiscountValue],
        p.[IsActive], p.[CreatedAt],
        c.[Name] AS [CategoryName]
    FROM [dbo].[Products] p
    LEFT JOIN [dbo].[Categories] c ON c.[Id] = p.[CategoryId]
    WHERE (@OnlyActive = 0 OR p.[IsActive] = 1)
      AND (@CategoryId IS NULL OR p.[CategoryId] = @CategoryId)
      AND (
            @Q IS NULL
            OR p.[Name] LIKE '%' + @Q + '%'
            OR p.[ModelNo] LIKE '%' + @Q + '%'
            OR p.[SerialNo] LIKE '%' + @Q + '%'
          )
    ORDER BY p.[Name];
END

