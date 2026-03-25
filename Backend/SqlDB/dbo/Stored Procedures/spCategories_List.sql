CREATE PROCEDURE [dbo].[spCategories_List]
    @OnlyActive BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [Name], [IsActive], [CreatedAt]
    FROM [dbo].[Categories]
    WHERE (@OnlyActive = 0 OR [IsActive] = 1)
    ORDER BY [Name];
END

