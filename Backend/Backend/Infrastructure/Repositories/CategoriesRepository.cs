using System.Data;
using Dapper;
using Backend.Domain.Entities;
using Backend.Infrastructure.Db;

namespace Backend.Infrastructure.Repositories;

public interface ICategoriesRepository
{
    Task<IReadOnlyList<Category>> ListAsync(bool onlyActive, CancellationToken cancellationToken);
    Task<Category?> GetAsync(int id, CancellationToken cancellationToken);
    Task<int> CreateAsync(string name, bool isActive, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(int id, string name, bool isActive, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}

public sealed class CategoriesRepository(IDbConnectionFactory connectionFactory) : ICategoriesRepository
{
    public async Task<IReadOnlyList<Category>> ListAsync(bool onlyActive, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<Category>(
            new CommandDefinition(
                "[dbo].[spCategories_List]",
                new { OnlyActive = onlyActive },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<Category?> GetAsync(int id, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Category>(
            new CommandDefinition(
                "[dbo].[spCategories_Get]",
                new { Id = id },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
    }

    public async Task<int> CreateAsync(string name, bool isActive, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                "[dbo].[spCategories_Create]",
                new { Name = name, IsActive = isActive },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(int id, string name, bool isActive, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        var affected = await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                "[dbo].[spCategories_Update]",
                new { Id = id, Name = name, IsActive = isActive },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        var affected = await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                "[dbo].[spCategories_Delete]",
                new { Id = id },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
        return affected > 0;
    }
}
