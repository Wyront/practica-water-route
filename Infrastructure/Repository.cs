using MySqlConnector;
using WaterRoute.Models;

namespace WaterRoute.Infrastructure;

public sealed class Repository : IRepository, IDisposable
{
    private readonly IDbConnectionFactory _factory;
    private MySqlConnection? _sharedConnection;
    private MySqlTransaction? _transaction;
    private bool _disposed;

    public Repository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    // Для транзакционных операций из сервисов
    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _sharedConnection ??= await _factory.CreateConnectionAsync(ct);
        _transaction = await _sharedConnection.BeginTransactionAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task ExecuteAsync(string procedureName, object? parameters = null, CancellationToken ct = default)
    {
        await using var cmd = CreateCommand(procedureName, parameters);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<T?> QuerySingleAsync<T>(string procedureName, object? parameters = null, CancellationToken ct = default) where T : class
    {
        await using var cmd = CreateCommand(procedureName, parameters);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
            return MapRow<T>(reader);
        return null;
    }

    public async Task<IReadOnlyList<T>> QueryAsync<T>(string procedureName, object? parameters = null, CancellationToken ct = default) where T : class
    {
        var list = new List<T>();
        await using var cmd = CreateCommand(procedureName, parameters);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
            list.Add(MapRow<T>(reader));
        return list;
    }

    public async Task<MultiResultSet> QueryMultipleAsync(string procedureName, object? parameters = null, CancellationToken ct = default)
    {
        var cmd = CreateCommand(procedureName, parameters);
        var reader = await cmd.ExecuteReaderAsync(ct);
        return new MultiResultSet(reader);
    }

    private MySqlCommand CreateCommand(string procedureName, object? parameters)
    {
        var conn = _sharedConnection ?? throw new InvalidOperationException("Нет активного соединения. Вызовите BeginTransactionAsync или используйте ExecuteAsync без транзакции.");
        var cmd = conn.CreateCommand();
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = procedureName;
        cmd.Transaction = _transaction;

        if (parameters != null)
        {
            foreach (var prop in parameters.GetType().GetProperties())
            {
                var value = prop.GetValue(parameters);
                var param = cmd.CreateParameter();
                param.ParameterName = $"p_{prop.Name}";
                param.Value = value ?? DBNull.Value;
                cmd.Parameters.Add(param);
            }
        }

        return cmd;
    }

    private static T MapRow<T>(MySqlDataReader reader) where T : class
    {
        var obj = Activator.CreateInstance<T>();
        var props = typeof(T).GetProperties();
        for (int i = 0; i < reader.FieldCount; i++)
        {
            var name = reader.GetName(i);
            var prop = Array.Find(props, p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
            if (prop != null && prop.CanWrite && !reader.IsDBNull(i))
            {
                var value = reader.GetValue(i);
                prop.SetValue(obj, value);
            }
        }
        return obj;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _transaction?.Dispose();
            _sharedConnection?.Dispose();
            _disposed = true;
        }
    }
}