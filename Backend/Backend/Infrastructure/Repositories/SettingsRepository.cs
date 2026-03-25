using System.Data;
using Dapper;
using Backend.Domain.Entities;
using Backend.Infrastructure.Db;

namespace Backend.Infrastructure.Repositories;

public interface ISettingsRepository
{
    Task<Setting> GetAsync(CancellationToken cancellationToken);
    Task<Setting> UpsertAsync(object parameters, CancellationToken cancellationToken);
}

public sealed class SettingsRepository(IDbConnectionFactory connectionFactory) : ISettingsRepository
{
    public async Task<Setting> GetAsync(CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleAsync<Setting>(
            new CommandDefinition(
                "[dbo].[spSettings_Get]",
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
    }

    public async Task<Setting> UpsertAsync(object parameters, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleAsync<Setting>(
            new CommandDefinition(
                "[dbo].[spSettings_Upsert]",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
    }
}
