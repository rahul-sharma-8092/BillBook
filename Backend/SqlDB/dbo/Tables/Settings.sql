CREATE TABLE [dbo].[Settings] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [ShopName]      NVARCHAR (200) NULL,
    [Address]       NVARCHAR (500) NULL,
    [Phone]         NVARCHAR (50)  NULL,
    [Email]         NVARCHAR (150) NULL,
    [BankName]      NVARCHAR (150) NULL,
    [AccountName]   NVARCHAR (150) NULL,
    [AccountNumber] NVARCHAR (50)  NULL,
    [IFSC]          NVARCHAR (50)  NULL,
    [UPI]           NVARCHAR (100) NULL,
    [Terms]         NVARCHAR (MAX) NULL,
    [FooterMessage] NVARCHAR (500) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

