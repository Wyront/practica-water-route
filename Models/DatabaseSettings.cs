using MySqlConnector;

namespace WaterRoute.Models;

public sealed class DatabaseSettings
{
    public string Server { get; init; } = "localhost";
    public uint Port { get; init; } = 3306;
    public string Database { get; init; } = "waterRoute";
    public string UserId { get; init; } = "root";
    public string Password { get; init; } = "";
    public bool SslMode { get; init; } = false;

    // MySqlConnectionStringBuilder сам экранирует спецсимволы (например ';' в пароле).
    public string ToConnectionString()
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Server = Server,
            Port = Port,
            Database = Database,
            UserID = UserId,
            Password = Password,
            SslMode = SslMode ? MySqlSslMode.Required : MySqlSslMode.None,
            CharacterSet = "utf8mb4",
            AllowUserVariables = true,
            ConnectionTimeout = 10,
            DefaultCommandTimeout = 30
        };
        // Collation is set via session variable after connection
        return builder.ConnectionString;
    }
}
