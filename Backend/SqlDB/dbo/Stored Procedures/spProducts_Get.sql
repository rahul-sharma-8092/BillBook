CREATE PROCEDURE [dbo].[spProducts_Get]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.[Id], p.[CategoryId], p.[ProductType], p.[Name], p.[ModelNo], p.[SerialNo],
        p.[WarrantyMonths], p.[WarrantyNote], p.[InvoiceNote],
        p.[PurchasePrice], p.[SellPrice], p.[StockQty],
        p.[DiscountType], p.[DiscountValue],
        p.[IsActive], p.[CreatedAt]
    FROM [dbo].[Products] p
    WHERE p.[Id] = @Id;
END

