CREATE PROCEDURE [dbo].[spCategories_Create]
    @Name NVARCHAR(150),
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Categories] ([Name], [IsActive])
    VALUES (@Name, @IsActive);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS [Id];
END

