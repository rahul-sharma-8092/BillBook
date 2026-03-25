CREATE PROCEDURE [dbo].[spSettings_Upsert]
    @ShopName NVARCHAR(200) = NULL,
    @Address NVARCHAR(500) = NULL,
    @Phone NVARCHAR(50) = NULL,
    @Email NVARCHAR(150) = NULL,
    @BankName NVARCHAR(150) = NULL,
    @AccountName NVARCHAR(150) = NULL,
    @AccountNumber NVARCHAR(50) = NULL,
    @IFSC NVARCHAR(50) = NULL,
    @UPI NVARCHAR(100) = NULL,
    @Terms NVARCHAR(MAX) = NULL,
    @FooterMessage NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Settings])
    BEGIN
        INSERT INTO [dbo].[Settings] (
            [ShopName], [Address], [Phone], [Email],
            [BankName], [AccountName], [AccountNumber], [IFSC], [UPI],
            [Terms], [FooterMessage]
        )
        VALUES (
            @ShopName, @Address, @Phone, @Email,
            @BankName, @AccountName, @AccountNumber, @IFSC, @UPI,
            @Terms, @FooterMessage
        );
    END
    ELSE
    BEGIN
        UPDATE s
        SET
            [ShopName] = @ShopName,
            [Address] = @Address,
            [Phone] = @Phone,
            [Email] = @Email,
            [BankName] = @BankName,
            [AccountName] = @AccountName,
            [AccountNumber] = @AccountNumber,
            [IFSC] = @IFSC,
            [UPI] = @UPI,
            [Terms] = @Terms,
            [FooterMessage] = @FooterMessage
        FROM [dbo].[Settings] s
        WHERE s.[Id] = (SELECT TOP (1) [Id] FROM [dbo].[Settings] ORDER BY [Id]);
    END

    EXEC [dbo].[spSettings_Get];
END

