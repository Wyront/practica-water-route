using System.Data;
using System.Dynamic;
using System.Globalization;
using MySqlConnector;

namespace WaterRoute.Infrastructure;

public sealed class Repository : IRepository, IDisposable
{
    private readonly IDbConnectionFactory _factory;
    private MySqlConnection? _sharedConnection;
    private MySqlTransaction? _transaction;
    private bool _disposed;

    public Repository(IDbConnectionFactory factory) => _factory = factory;

    // ===== Транзакции =====
    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
            throw new InvalidOperationException("Транзакция уже открыта.");
        _sharedConnection ??= await _factory.CreateConnectionAsync(ct);
        _transaction = await _sharedConnection.BeginTransactionAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_transaction == null) return;
        await _transaction.CommitAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction == null) return;
        try { await _transaction.RollbackAsync(ct); }
        finally { await _transaction.DisposeAsync(); _transaction = null; }
    }

    // ===== Вызовы без возврата данных =====
    public async Task ExecuteAsync(string procedureName, object? parameters = null, CancellationToken ct = default)
    {
        var (cmd, ownConnection) = await CreateCommandAsync(procedureName, parameters, null, ct);
        await using (cmd)
        await using (ownConnection)
        {
            await cmd.ExecuteNonQueryAsync(ct);
        }
    }

    public async Task<IReadOnlyDictionary<string, object?>> ExecuteWithOutputsAsync(
        string procedureName, object? parameters = null, string[]? outputParameters = null, CancellationToken ct = default)
    {
        var (cmd, ownConnection) = await CreateCommandAsync(procedureName, parameters, outputParameters, ct);
        await using (cmd)
        await using (ownConnection)
        {
            await cmd.ExecuteNonQueryAsync(ct);
            var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            foreach (var name in outputParameters ?? Array.Empty<string>())
            {
                var key = NormalizeName(name);
                var value = cmd.Parameters[key].Value;
                result[key] = value is DBNull ? null : value;
            }
            return result;
        }
    }

    // ===== Выборки =====
    public async Task<T?> QuerySingleAsync<T>(string procedureName, object? parameters = null, CancellationToken ct = default) where T : class
    {
        var (cmd, ownConnection) = await CreateCommandAsync(procedureName, parameters, null, ct);
        await using (cmd)
        await using (ownConnection)
        await using (var reader = await cmd.ExecuteReaderAsync(ct))
        {
            return await reader.ReadAsync(ct) ? MapRow<T>(reader) : null;
        }
    }

    public async Task<IReadOnlyList<T>> QueryAsync<T>(string procedureName, object? parameters = null, CancellationToken ct = default) where T : class
    {
        var (cmd, ownConnection) = await CreateCommandAsync(procedureName, parameters, null, ct);
        await using (cmd)
        await using (ownConnection)
        await using (var reader = await cmd.ExecuteReaderAsync(ct))
        {
            var list = new List<T>();
            while (await reader.ReadAsync(ct))
                list.Add(MapRow<T>(reader));
            return list;
        }
    }

    public async Task<(IReadOnlyList<T> Rows, int Total)> QueryPagedAsync<T>(string procedureName, object? parameters = null, CancellationToken ct = default) where T : class
    {
        var (cmd, ownConnection) = await CreateCommandAsync(procedureName, parameters, null, ct);
        await using (cmd)
        await using (ownConnection)
        await using (var reader = await cmd.ExecuteReaderAsync(ct))
        {
            var rows = new List<T>();
            while (await reader.ReadAsync(ct))
                rows.Add(MapRow<T>(reader));

            var total = rows.Count;
            if (await reader.NextResultAsync(ct) && await reader.ReadAsync(ct))
                total = Convert.ToInt32(reader.GetValue(0), CultureInfo.InvariantCulture);
            return (rows, total);
        }
    }

    public async Task<MultiResultSet> QueryMultipleAsync(string procedureName, object? parameters = null, CancellationToken ct = default)
    {
        var (cmd, ownConnection) = await CreateCommandAsync(procedureName, parameters, null, ct);
        var reader = await cmd.ExecuteReaderAsync(ct);
        return new MultiResultSet(reader, cmd, ownConnection);
    }

    // ===== Инфраструктура =====
    private async Task<(MySqlCommand Cmd, MySqlConnection? OwnConnection)> CreateCommandAsync(
        string procedureName, object? parameters, string[]? outputParameters, CancellationToken ct)
    {
        // Нет транзакции — открываем отдельное соединение на время вызова (и закрываем после)
        MySqlConnection? ownConnection = null;
        var conn = _sharedConnection;
        if (conn is null)
        {
            ownConnection = await _factory.CreateConnectionAsync(ct);
            conn = ownConnection;
        }

        var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;   // MySqlConnector сам строит CALL sp_...
        cmd.CommandText = procedureName;
        cmd.Transaction = _transaction;

        if (parameters != null)
        {
            foreach (var prop in parameters.GetType().GetProperties())
            {
                var param = cmd.CreateParameter();
                param.ParameterName = NormalizeName(prop.Name); // "email" -> "p_email", "p_email" -> "p_email"
                param.Value = prop.GetValue(parameters) ?? DBNull.Value;
                cmd.Parameters.Add(param);
            }
        }

        if (outputParameters != null)
        {
            foreach (var name in outputParameters)
            {
                var key = NormalizeName(name);
                if (cmd.Parameters.Contains(key)) continue;
                var param = cmd.CreateParameter();
                param.ParameterName = key;
                param.Direction = ParameterDirection.Output; // MySqlDbTypeconnector возьмёт из метаданных процедуры
                cmd.Parameters.Add(param);
            }
        }

        return (cmd, ownConnection);
    }

    private static string NormalizeName(string name) =>
        name.StartsWith("p_", StringComparison.OrdinalIgnoreCase) ? name : "p_" + name;

    private static T MapRow<T>(MySqlDataReader reader) where T : class
    {
        // Поддержка dynamic: QuerySingleAsync<dynamic> / QueryAsync<dynamic>
        if (typeof(T) == typeof(object))
        {
            IDictionary<string, object?> row = new ExpandoObject();
            for (var i = 0; i < reader.FieldCount; i++)
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            return (T)(object)row;
        }

        var obj = Activator.CreateInstance<T>();
        var props = typeof(T).GetProperties();
        for (var i = 0; i < reader.FieldCount; i++)
        {
            if (reader.IsDBNull(i)) continue;
            var name = reader.GetName(i);
            // full_name -> FullName, role_code -> RoleCode и т.п.
            var prop = Array.Find(props, p => p.CanWrite &&
                (string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(p.Name, name.Replace("_", ""), StringComparison.OrdinalIgnoreCase)));
            if (prop == null) continue;

            try { prop.SetValue(obj, ConvertValue(reader.GetValue(i), prop.PropertyType)); }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Не удалось смапить колонку '{name}' в {typeof(T).Name}.{prop.Name}: {ex.Message}", ex);
            }
        }
        return obj;
    }

    private static object? ConvertValue(object value, Type targetType)
    {
        var type = Nullable.GetUnderlyingType(targetType) ?? targetType;
        if (type.IsInstanceOfType(value)) return value;
        if (type.IsEnum) return value is string s ? Enum.Parse(type, s, true) : Enum.ToObject(type, value);
        if (type == typeof(Guid)) return Guid.Parse(value.ToString()!);
        if (type == typeof(bool)) return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
        if (type == typeof(string)) return value.ToString();
        return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _transaction?.Dispose();
        _sharedConnection?.Dispose();
        _disposed = true;
    }
}