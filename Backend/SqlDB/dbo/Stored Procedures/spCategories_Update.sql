CREATE PROCEDURE [dbo].[spCategories_Update]
    @Id INT,
    @Name NVARCHAR(150),
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Categories]
    SET [Name] = @Name,
        [IsActive] = @IsActive
    WHERE [Id] = @Id;

    SELECT @@ROWCOUNT AS [Affected];
END

