using System.Data;

namespace Backend.Infrastructure.Db;

public interface IDbConnectionFactory
{
    ValueTask<IDbConnection> OpenConnectionAsync(CancellationToken cancellationToken);
}

