CREATE PROCEDURE [dbo].[spCustomers_Update]
    @Id INT,
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

    UPDATE [dbo].[Customers]
    SET
        [Name] = @Name,
        [CompanyName] = @CompanyName,
        [Mobile] = @Mobile,
        [Email] = @Email,
        [AddressLine1] = @AddressLine1,
        [AddressLine2] = @AddressLine2,
        [City] = @City,
        [District] = @District,
        [State] = @State,
        [Country] = @Country,
        [Notes] = @Notes
    WHERE [Id] = @Id;

    SELECT @@ROWCOUNT AS [Affected];
END

