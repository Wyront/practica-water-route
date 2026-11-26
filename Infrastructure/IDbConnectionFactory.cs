using MySqlConnector;
using WaterRoute.Models;

namespace WaterRoute.Infrastructure;

public interface IDbConnectionFactory
{
    MySqlConnection CreateConnection();
    ValueTask<MySqlConnection> CreateConnectionAsync(CancellationToken ct = default);
}

public sealed class DbConnectionFactory : IDbConnectionFactory
{
    private readonly DatabaseSettings _settings;

    public DbConnectionFactory(DatabaseSettings settings)
    {
        _settings = settings;
    }

    public MySqlConnection CreateConnection()
    {
        var conn = new MySqlConnection(_settings.ToConnectionString());
        conn.Open();
        // Включаем проверку внешних ключей на каждом соединении
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SET FOREIGN_KEY_CHECKS=1;";
        cmd.ExecuteNonQuery();
        return conn;
    }

    public async ValueTask<MySqlConnection> CreateConnectionAsync(CancellationToken ct = default)
    {
        var conn = new MySqlConnection(_settings.ToConnectionString());
        await conn.OpenAsync(ct);
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SET FOREIGN_KEY_CHECKS=1;";
        await cmd.ExecuteNonQueryAsync(ct);
        return conn;
    }
}