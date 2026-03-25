CREATE PROCEDURE [dbo].[spProducts_Create]
    @CategoryId INT = NULL,
    @ProductType NVARCHAR(20),
    @Name NVARCHAR(200),
    @ModelNo NVARCHAR(100) = NULL,
    @SerialNo NVARCHAR(100) = NULL,
    @WarrantyMonths INT = NULL,
    @WarrantyNote NVARCHAR(300) = NULL,
    @InvoiceNote NVARCHAR(500) = NULL,
    @PurchasePrice DECIMAL(10,2) = NULL,
    @SellPrice DECIMAL(10,2),
    @StockQty DECIMAL(10,2) = 0,
    @DiscountType NVARCHAR(10) = NULL,
    @DiscountValue DECIMAL(10,2) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Products] (
        [CategoryId], [ProductType], [Name], [ModelNo], [SerialNo],
        [WarrantyMonths], [WarrantyNote], [InvoiceNote],
        [PurchasePrice], [SellPrice], [StockQty],
        [DiscountType], [DiscountValue],
        [IsActive]
    )
    VALUES (
        @CategoryId, @ProductType, @Name, @ModelNo, @SerialNo,
        @WarrantyMonths, @WarrantyNote, @InvoiceNote,
        @PurchasePrice, @SellPrice, @StockQty,
        @DiscountType, @DiscountValue,
        @IsActive
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS [Id];
END

