CREATE PROCEDURE [dbo].[spInvoices_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id], [InvoiceNo], [CustomerId], [CustomerName], [Mobile],
        [SubTotal], [DiscountType], [DiscountValue], [DiscountAmount],
        [TotalAmount], [PaidAmount], [BalanceAmount],
        [PaymentMode], [Notes], [CreatedAt]
    FROM [dbo].[Invoices]
    WHERE [Id] = @Id;

    SELECT
        [Id], [InvoiceId], [ProductId],
        [ProductName], [ProductType], [ModelNo], [SerialNo],
        [WarrantyMonths], [WarrantyNote],
        [Qty], [Rate],
        [DiscountType], [DiscountValue], [DiscountAmount],
        [Amount], [InvoiceNote]
    FROM [dbo].[InvoiceItems]
    WHERE [InvoiceId] = @Id
    ORDER BY [Id];
END

