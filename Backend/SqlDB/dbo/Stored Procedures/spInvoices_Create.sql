CREATE PROCEDURE [dbo].[spInvoices_Create]
    @CustomerId INT = NULL,
    @CustomerName NVARCHAR(200) = NULL,
    @Mobile NVARCHAR(20) = NULL,
    @DiscountType NVARCHAR(10) = NULL,
    @DiscountValue DECIMAL(10,2) = NULL,
    @PaidAmount DECIMAL(10,2) = 0,
    @PaymentMode NVARCHAR(50) = NULL,
    @Notes NVARCHAR(500) = NULL,
    @Items [dbo].[InvoiceItemType] READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @InvoiceId INT;
    DECLARE @InvoiceNo NVARCHAR(50);
    DECLARE @Year NVARCHAR(4) = CONVERT(NVARCHAR(4), YEAR(GETDATE()));
    DECLARE @Prefix NVARCHAR(20) = N'RK-' + @Year + N'-';
    DECLARE @NextSeq INT;

    BEGIN TRANSACTION;

    SELECT @NextSeq =
        ISNULL(
            MAX(TRY_CAST(RIGHT([InvoiceNo], 4) AS INT)),
            0
        ) + 1
    FROM [dbo].[Invoices] WITH (UPDLOCK, HOLDLOCK)
    WHERE [InvoiceNo] LIKE @Prefix + N'%';

    SET @InvoiceNo = @Prefix + RIGHT(N'0000' + CONVERT(NVARCHAR(10), @NextSeq), 4);

    ;WITH ComputedItems AS (
        SELECT
            i.[ProductId],
            i.[ProductName],
            i.[ProductType],
            i.[ModelNo],
            i.[SerialNo],
            i.[WarrantyMonths],
            i.[WarrantyNote],
            i.[Qty],
            i.[Rate],
            i.[DiscountType],
            i.[DiscountValue],
            CAST(
                CASE
                    WHEN i.[DiscountType] = N'%' AND i.[DiscountValue] IS NOT NULL THEN (i.[Qty] * i.[Rate]) * (i.[DiscountValue] / 100.0)
                    WHEN i.[DiscountType] = N'RS' AND i.[DiscountValue] IS NOT NULL THEN i.[DiscountValue]
                    ELSE 0
                END
            AS DECIMAL(10,2)) AS [DiscountAmount],
            CAST(
                (i.[Qty] * i.[Rate]) -
                CASE
                    WHEN i.[DiscountType] = N'%' AND i.[DiscountValue] IS NOT NULL THEN (i.[Qty] * i.[Rate]) * (i.[DiscountValue] / 100.0)
                    WHEN i.[DiscountType] = N'RS' AND i.[DiscountValue] IS NOT NULL THEN i.[DiscountValue]
                    ELSE 0
                END
            AS DECIMAL(10,2)) AS [Amount],
            i.[InvoiceNote]
        FROM @Items i
    )
    SELECT 1; -- keep CTE scope in SSDT happy

    DECLARE @SubTotal DECIMAL(10,2) =
        (SELECT ISNULL(SUM([Amount]), 0) FROM (
            SELECT
                CAST(
                    (i.[Qty] * i.[Rate]) -
                    CASE
                        WHEN i.[DiscountType] = N'%' AND i.[DiscountValue] IS NOT NULL THEN (i.[Qty] * i.[Rate]) * (i.[DiscountValue] / 100.0)
                        WHEN i.[DiscountType] = N'RS' AND i.[DiscountValue] IS NOT NULL THEN i.[DiscountValue]
                        ELSE 0
                    END
                AS DECIMAL(10,2)) AS [Amount]
            FROM @Items i
        ) x);

    DECLARE @InvoiceDiscountAmount DECIMAL(10,2) =
        CAST(
            CASE
                WHEN @DiscountType = N'%' AND @DiscountValue IS NOT NULL THEN @SubTotal * (@DiscountValue / 100.0)
                WHEN @DiscountType = N'RS' AND @DiscountValue IS NOT NULL THEN @DiscountValue
                ELSE 0
            END
        AS DECIMAL(10,2));

    DECLARE @TotalAmount DECIMAL(10,2) = CAST(@SubTotal - @InvoiceDiscountAmount AS DECIMAL(10,2));
    DECLARE @BalanceAmount DECIMAL(10,2) = CAST(@TotalAmount - ISNULL(@PaidAmount, 0) AS DECIMAL(10,2));

    INSERT INTO [dbo].[Invoices] (
        [InvoiceNo],
        [CustomerId],
        [CustomerName],
        [Mobile],
        [SubTotal],
        [DiscountType],
        [DiscountValue],
        [DiscountAmount],
        [TotalAmount],
        [PaidAmount],
        [BalanceAmount],
        [PaymentMode],
        [Notes]
    )
    VALUES (
        @InvoiceNo,
        @CustomerId,
        @CustomerName,
        @Mobile,
        @SubTotal,
        @DiscountType,
        @DiscountValue,
        @InvoiceDiscountAmount,
        @TotalAmount,
        ISNULL(@PaidAmount, 0),
        @BalanceAmount,
        @PaymentMode,
        @Notes
    );

    SET @InvoiceId = CAST(SCOPE_IDENTITY() AS INT);

    INSERT INTO [dbo].[InvoiceItems] (
        [InvoiceId],
        [ProductId],
        [ProductName],
        [ProductType],
        [ModelNo],
        [SerialNo],
        [WarrantyMonths],
        [WarrantyNote],
        [Qty],
        [Rate],
        [DiscountType],
        [DiscountValue],
        [DiscountAmount],
        [Amount],
        [InvoiceNote]
    )
    SELECT
        @InvoiceId,
        i.[ProductId],
        i.[ProductName],
        i.[ProductType],
        i.[ModelNo],
        i.[SerialNo],
        i.[WarrantyMonths],
        i.[WarrantyNote],
        i.[Qty],
        i.[Rate],
        i.[DiscountType],
        i.[DiscountValue],
        CAST(
            CASE
                WHEN i.[DiscountType] = N'%' AND i.[DiscountValue] IS NOT NULL THEN (i.[Qty] * i.[Rate]) * (i.[DiscountValue] / 100.0)
                WHEN i.[DiscountType] = N'RS' AND i.[DiscountValue] IS NOT NULL THEN i.[DiscountValue]
                ELSE 0
            END
        AS DECIMAL(10,2)) AS [DiscountAmount],
        CAST(
            (i.[Qty] * i.[Rate]) -
            CASE
                WHEN i.[DiscountType] = N'%' AND i.[DiscountValue] IS NOT NULL THEN (i.[Qty] * i.[Rate]) * (i.[DiscountValue] / 100.0)
                WHEN i.[DiscountType] = N'RS' AND i.[DiscountValue] IS NOT NULL THEN i.[DiscountValue]
                ELSE 0
            END
        AS DECIMAL(10,2)) AS [Amount],
        i.[InvoiceNote]
    FROM @Items i;

    COMMIT TRANSACTION;

    SELECT @InvoiceId AS [InvoiceId], @InvoiceNo AS [InvoiceNo];
END

