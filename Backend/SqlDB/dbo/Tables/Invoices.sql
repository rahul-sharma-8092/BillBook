CREATE TABLE [dbo].[Invoices] (
    [Id]             INT             IDENTITY (1, 1) NOT NULL,
    [InvoiceNo]      NVARCHAR (50)   NOT NULL,
    [CustomerId]     INT             NULL,
    [CustomerName]   NVARCHAR (200)  NULL,
    [Mobile]         NVARCHAR (20)   NULL,
    [SubTotal]       DECIMAL (10, 2) NOT NULL,
    [DiscountType]   NVARCHAR (10)   NULL,
    [DiscountValue]  DECIMAL (10, 2) NULL,
    [DiscountAmount] DECIMAL (10, 2) NULL,
    [TotalAmount]    DECIMAL (10, 2) NOT NULL,
    [PaidAmount]     DECIMAL (10, 2) DEFAULT ((0)) NULL,
    [BalanceAmount]  DECIMAL (10, 2) DEFAULT ((0)) NULL,
    [PaymentMode]    NVARCHAR (50)   NULL,
    [Notes]          NVARCHAR (500)  NULL,
    [CreatedAt]      DATETIME        DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id])
);

