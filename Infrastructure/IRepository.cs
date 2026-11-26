using MySqlConnector;
using WaterRoute.Models;

namespace WaterRoute.Infrastructure;

public interface IRepository
{
    // Транзакции
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);

    // Вызов процедуры без возврата данных
    Task ExecuteAsync(string procedureName, object? parameters = null, CancellationToken ct = default);

    // Вызов процедуры, возвращающей один набор строк
    Task<T?> QuerySingleAsync<T>(string procedureName, object? parameters = null, CancellationToken ct = default) where T : class;

    // Вызов процедуры, возвращающей список строк
    Task<IReadOnlyList<T>> QueryAsync<T>(string procedureName, object? parameters = null, CancellationToken ct = default) where T : class;

    // Вызов процедуры с множественными наборами результатов
    Task<MultiResultSet> QueryMultipleAsync(string procedureName, object? parameters = null, CancellationToken ct = default);
}

public sealed class MultiResultSet : IDisposable
{
    private readonly MySqlDataReader _reader;
    private bool _disposed;

    internal MultiResultSet(MySqlDataReader reader) => _reader = reader;

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
        if (!_disposed)
        {
            _reader.Dispose();
            _disposed = true;
        }
    }
}