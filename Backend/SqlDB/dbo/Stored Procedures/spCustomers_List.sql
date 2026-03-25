CREATE PROCEDURE [dbo].[spCustomers_List]
    @Q NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (50)
        [Id], [Name], [CompanyName], [Mobile], [Email],
        [AddressLine1], [AddressLine2], [City], [District], [State], [Country],
        [Notes], [CreatedAt]
    FROM [dbo].[Customers]
    WHERE
        @Q IS NULL
        OR [Name] LIKE '%' + @Q + '%'
        OR [Mobile] LIKE '%' + @Q + '%'
        OR [CompanyName] LIKE '%' + @Q + '%'
    ORDER BY [Name];
END

