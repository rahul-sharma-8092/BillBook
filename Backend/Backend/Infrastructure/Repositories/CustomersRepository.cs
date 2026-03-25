using System.Data;
using Dapper;
using Backend.Domain.Entities;
using Backend.Infrastructure.Db;

namespace Backend.Infrastructure.Repositories;

public interface ICustomersRepository
{
    Task<IReadOnlyList<Customer>> ListAsync(string? q, CancellationToken cancellationToken);
    Task<Customer?> GetAsync(int id, CancellationToken cancellationToken);
    Task<int> CreateAsync(object parameters, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(object parameters, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}

public sealed class CustomersRepository(IDbConnectionFactory connectionFactory) : ICustomersRepository
{
    public async Task<IReadOnlyList<Customer>> ListAsync(string? q, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<Customer>(
            new CommandDefinition(
                "[dbo].[spCustomers_List]",
                new { Q = q },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<Customer?> GetAsync(int id, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Customer>(
            new CommandDefinition(
                "[dbo].[spCustomers_Get]",
                new { Id = id },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
    }

    public async Task<int> CreateAsync(object parameters, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                "[dbo].[spCustomers_Create]",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(object parameters, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        var affected = await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                "[dbo].[spCustomers_Update]",
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
                "[dbo].[spCustomers_Delete]",
                new { Id = id },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
        return affected > 0;
    }
}
