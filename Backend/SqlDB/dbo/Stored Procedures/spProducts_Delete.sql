CREATE PROCEDURE [dbo].[spProducts_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[Products]
    WHERE [Id] = @Id;

    SELECT @@ROWCOUNT AS [Affected];
END

