CREATE TABLE [dbo].[InvoiceItems] (
    [Id]             INT             IDENTITY (1, 1) NOT NULL,
    [InvoiceId]      INT             NOT NULL,
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
    [DiscountAmount] DECIMAL (10, 2) NULL,
    [Amount]         DECIMAL (10, 2) NOT NULL,
    [InvoiceNote]    NVARCHAR (500)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([InvoiceId]) REFERENCES [dbo].[Invoices] ([Id])
);

