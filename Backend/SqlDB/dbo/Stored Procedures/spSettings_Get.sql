CREATE PROCEDURE [dbo].[spSettings_Get]
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Settings])
    BEGIN
        INSERT INTO [dbo].[Settings] ([ShopName]) VALUES (NULL);
    END

    SELECT TOP (1)
        [Id], [ShopName], [Address], [Phone], [Email],
        [BankName], [AccountName], [AccountNumber], [IFSC], [UPI],
        [Terms], [FooterMessage]
    FROM [dbo].[Settings]
    ORDER BY [Id];
END

