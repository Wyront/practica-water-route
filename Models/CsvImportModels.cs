namespace WaterRoute.Models;

public enum CsvImportType
{
    StockReceipts,
    Products
}

public sealed record CsvImportError(int LineNumber, string Field, string Message);

public sealed record CsvPreviewRow(
    int LineNumber,
    string Warehouse,
    string Sku,
    string Quantity,
    string Result,
    bool IsError);

public sealed class CsvImportResult
{
    public required CsvImportType ImportType { get; init; }
    public List<CsvPreviewRow> PreviewRows { get; } = [];
    public List<CsvImportError> Errors { get; } = [];
    public List<IReadOnlyDictionary<string, string>> ValidRows { get; } = [];
    public bool IsValid => Errors.Count == 0 && ValidRows.Count > 0;
    public string StatusText => Errors.Count > 0
        ? $"Найдено ошибок: {Errors.Count}. Импорт заблокирован."
        : ValidRows.Count == 0
            ? "Файл не содержит данных для импорта."
            : $"Проверка пройдена: {ValidRows.Count} строк готово к импорту.";
}
