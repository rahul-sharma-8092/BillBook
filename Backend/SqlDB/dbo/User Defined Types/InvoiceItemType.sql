CREATE TYPE [dbo].[InvoiceItemType] AS TABLE (
    [ProductId]      INT             NULL,
    [ProductName]    NVARCHAR (200)  NOT NULL,
    [ProductType]    NVARCHAR (20)   NULL,
    [ModelNo]        NVARCHAR (100)  NULL,
    [SerialNo]       NVARCHAR (100)  NULL,
    [WarrantyMonths] INT             NULL,
    [WarrantyNote]   NVARCHAR (300)  NULL,
    [Qty]            DECIMAL (10, 2) NOT NULL,
    [Rate]           DECIMAL (10, 2) NOT NULL,
    [DiscountType]   NVARCHAR (10)   NULL,
    [DiscountValue]  DECIMAL (10, 2) NULL,
    [InvoiceNote]    NVARCHAR (500)  NULL
);

