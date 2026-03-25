CREATE PROCEDURE [dbo].[spProducts_Update]
    @Id INT,
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
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Products]
    SET
        [CategoryId] = @CategoryId,
        [ProductType] = @ProductType,
        [Name] = @Name,
        [ModelNo] = @ModelNo,
        [SerialNo] = @SerialNo,
        [WarrantyMonths] = @WarrantyMonths,
        [WarrantyNote] = @WarrantyNote,
        [InvoiceNote] = @InvoiceNote,
        [PurchasePrice] = @PurchasePrice,
        [SellPrice] = @SellPrice,
        [StockQty] = @StockQty,
        [DiscountType] = @DiscountType,
        [DiscountValue] = @DiscountValue,
        [IsActive] = @IsActive
    WHERE [Id] = @Id;

    SELECT @@ROWCOUNT AS [Affected];
END

