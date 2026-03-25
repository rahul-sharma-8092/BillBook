CREATE TABLE [dbo].[Customers] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (200) NOT NULL,
    [CompanyName]  NVARCHAR (200) NULL,
    [Mobile]       NVARCHAR (20)  NULL,
    [Email]        NVARCHAR (150) NULL,
    [AddressLine1] NVARCHAR (300) NULL,
    [AddressLine2] NVARCHAR (300) NULL,
    [City]         NVARCHAR (100) NULL,
    [District]     NVARCHAR (100) NULL,
    [State]        NVARCHAR (100) NULL,
    [Country]      NVARCHAR (100) DEFAULT ('India') NULL,
    [Notes]        NVARCHAR (500) NULL,
    [CreatedAt]    DATETIME       DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

