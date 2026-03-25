CREATE PROCEDURE [dbo].[spCustomers_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[Customers]
    WHERE [Id] = @Id;

    SELECT @@ROWCOUNT AS [Affected];
END

