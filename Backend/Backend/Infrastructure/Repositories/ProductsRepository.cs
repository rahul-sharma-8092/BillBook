using System.Data;
using Dapper;
using Backend.Domain.Entities;
using Backend.Infrastructure.Db;

namespace Backend.Infrastructure.Repositories;

public interface IProductsRepository
{
    Task<IReadOnlyList<Product>> ListAsync(string? q, int? categoryId, bool onlyActive, CancellationToken cancellationToken);
    Task<Product?> GetAsync(int id, CancellationToken cancellationToken);
    Task<int> CreateAsync(object parameters, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(object parameters, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}

public sealed class ProductsRepository(IDbConnectionFactory connectionFactory) : IProductsRepository
{
    public async Task<IReadOnlyList<Product>> ListAsync(string? q, int? categoryId, bool onlyActive, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<Product>(
            new CommandDefinition(
                "[dbo].[spProducts_List]",
                new { Q = q, CategoryId = categoryId, OnlyActive = onlyActive },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<Product?> GetAsync(int id, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Product>(
            new CommandDefinition(
                "[dbo].[spProducts_Get]",
                new { Id = id },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
    }

    public async Task<int> CreateAsync(object parameters, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                "[dbo].[spProducts_Create]",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(object parameters, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        var affected = await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                "[dbo].[spProducts_Update]",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        var affected = await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                "[dbo].[spProducts_Delete]",
                new { Id = id },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
        return affected > 0;
    }
}
