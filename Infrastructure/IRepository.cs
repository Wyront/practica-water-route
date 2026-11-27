using MySqlConnector;

namespace WaterRoute.Infrastructure;

public interface IRepository
{
    // Транзакции (опционально: процедуры сами управляют транзакциями внутри себя)
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);

    // Вызов процедуры без возврата данных (работает и без открытой транзакции)
    Task ExecuteAsync(string procedureName, object? parameters = null, CancellationToken ct = default);

    // Вызов процедуры с OUT-параметрами: имена можно с "p_" или без
    Task<IReadOnlyDictionary<string, object?>> ExecuteWithOutputsAsync(
        string procedureName, object? parameters = null, string[]? outputParameters = null, CancellationToken ct = default);

    // Один набор строк
    Task<T?> QuerySingleAsync<T>(string procedureName, object? parameters = null, CancellationToken ct = default) where T : class;
    Task<IReadOnlyList<T>> QueryAsync<T>(string procedureName, object? parameters = null, CancellationToken ct = default) where T : class;

    // Списковые процедуры: 1-й набор — строки, 2-й — total_count (sp_user_list, sp_order_list, ...)
    Task<(IReadOnlyList<T> Rows, int Total)> QueryPagedAsync<T>(string procedureName, object? parameters = null, CancellationToken ct = default) where T : class;

    // Несколько наборов результатов (sp_monitor_get, sp_client_get, ...)
    Task<MultiResultSet> QueryMultipleAsync(string procedureName, object? parameters = null, CancellationToken ct = default);
}

public sealed class MultiResultSet : IDisposable
{
    private readonly MySqlDataReader _reader;
    private readonly MySqlCommand _cmd;
    private readonly MySqlConnection? _ownConnection;
    private bool _disposed;

    internal MultiResultSet(MySqlDataReader reader, MySqlCommand cmd, MySqlConnection? ownConnection)
    {
        _reader = reader;
        _cmd = cmd;
        _ownConnection = ownConnection;
    }

    public IReadOnlyList<T> ReadAll<T>(Func<MySqlDataReader, T> mapper) where T : class
    {
        var list = new List<T>();
        while (_reader.Read())
            list.Add(mapper(_reader));
        return list;
    }

    public bool NextResult() => _reader.NextResult();

    public void Dispose()
    {
        if (_disposed) return;
        _reader.Dispose();
        _cmd.Dispose();
        _ownConnection?.Dispose();
        _disposed = true;
    }
}