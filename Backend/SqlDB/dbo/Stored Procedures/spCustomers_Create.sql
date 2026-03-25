CREATE PROCEDURE [dbo].[spCustomers_Create]
    @Name NVARCHAR(200),
    @CompanyName NVARCHAR(200) = NULL,
    @Mobile NVARCHAR(20) = NULL,
    @Email NVARCHAR(150) = NULL,
    @AddressLine1 NVARCHAR(300) = NULL,
    @AddressLine2 NVARCHAR(300) = NULL,
    @City NVARCHAR(100) = NULL,
    @District NVARCHAR(100) = NULL,
    @State NVARCHAR(100) = NULL,
    @Country NVARCHAR(100) = 'India',
    @Notes NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Customers] (
        [Name], [CompanyName], [Mobile], [Email],
        [AddressLine1], [AddressLine2], [City], [District], [State], [Country],
        [Notes]
    )
    VALUES (
        @Name, @CompanyName, @Mobile, @Email,
        @AddressLine1, @AddressLine2, @City, @District, @State, @Country,
        @Notes
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS [Id];
END

