using System.Data;
using Dapper;
using Backend.Application.Dashboard;
using Backend.Infrastructure.Db;

namespace Backend.Infrastructure.Repositories;

public interface IDashboardRepository
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<SalesPointDto>> GetDailySalesAsync(int days, CancellationToken cancellationToken);
    Task<IReadOnlyList<MonthlyPointDto>> GetMonthlyTrendAsync(int months, CancellationToken cancellationToken);
}

public sealed class DashboardRepository(IDbConnectionFactory connectionFactory) : IDashboardRepository
{
    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleAsync<DashboardSummaryDto>(
            new CommandDefinition(
                "[dbo].[spDashboard_Summary]",
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<SalesPointDto>> GetDailySalesAsync(int days, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<SalesPointDto>(
            new CommandDefinition(
                "[dbo].[spDashboard_DailySales]",
                new { Days = days },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<IReadOnlyList<MonthlyPointDto>> GetMonthlyTrendAsync(int months, CancellationToken cancellationToken)
    {
        using var connection = (IDbConnection)await connectionFactory.OpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<MonthlyPointDto>(
            new CommandDefinition(
                "[dbo].[spDashboard_MonthlyTrend]",
                new { Months = months },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
        return rows.AsList();
    }
}
