CREATE TABLE [dbo].[Products] (
    [Id]             INT             IDENTITY (1, 1) NOT NULL,
    [CategoryId]     INT             NULL,
    [ProductType]    NVARCHAR (20)   NOT NULL,
    [Name]           NVARCHAR (200)  NOT NULL,
    [ModelNo]        NVARCHAR (100)  NULL,
    [SerialNo]       NVARCHAR (100)  NULL,
    [WarrantyMonths] INT             NULL,
    [WarrantyNote]   NVARCHAR (300)  NULL,
    [InvoiceNote]    NVARCHAR (500)  NULL,
    [PurchasePrice]  DECIMAL (10, 2) NULL,
    [SellPrice]      DECIMAL (10, 2) NOT NULL,
    [StockQty]       DECIMAL (10, 2) DEFAULT ((0)) NULL,
    [DiscountType]   NVARCHAR (10)   NULL,
    [DiscountValue]  DECIMAL (10, 2) NULL,
    [IsActive]       BIT             DEFAULT ((1)) NULL,
    [CreatedAt]      DATETIME        DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id])
);

