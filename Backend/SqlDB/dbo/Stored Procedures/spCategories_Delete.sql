CREATE PROCEDURE [dbo].[spCategories_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[Categories]
    WHERE [Id] = @Id;

    SELECT @@ROWCOUNT AS [Affected];
END

