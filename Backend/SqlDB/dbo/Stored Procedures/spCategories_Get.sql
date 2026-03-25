CREATE PROCEDURE [dbo].[spCategories_Get]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [Name], [IsActive], [CreatedAt]
    FROM [dbo].[Categories]
    WHERE [Id] = @Id;
END

