CREATE PROCEDURE [dbo].[spCustomers_Get]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id], [Name], [CompanyName], [Mobile], [Email],
        [AddressLine1], [AddressLine2], [City], [District], [State], [Country],
        [Notes], [CreatedAt]
    FROM [dbo].[Customers]
    WHERE [Id] = @Id;
END

