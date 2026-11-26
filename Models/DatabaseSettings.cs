namespace WaterRoute.Models;

public sealed class DatabaseSettings
{
    public string Server { get; init; } = "localhost";
    public uint Port { get; init; } = 3306;
    public string Database { get; init; } = "waterroute";
    public string UserId { get; init; } = "root";
    public string Password { get; init; } = "";
    public bool SslMode { get; init; } = false;

    public string ToConnectionString() =>
        $"Server={Server};Port={Port};Database={Database};User={UserId};Password={Password};" +
        $"SslMode={(SslMode ? "Required" : "None")};" +
        "CharSet=utf8mb4;AllowUserVariables=true;";
}