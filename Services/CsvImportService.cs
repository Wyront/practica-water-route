using WaterRoute.Models;

namespace WaterRoute.Services;

public sealed class CsvImportService
{
    private readonly CsvFileService _csv = new();
    private static readonly HashSet<string> Warehouses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Центральный склад", "Северный склад", "Приморский склад"
    };
    private static readonly HashSet<string> ExistingSkus = new(StringComparer.OrdinalIgnoreCase)
    {
        "WT-019", "WT-005", "WM-019", "AC-001", "WT-019-2", "BT-019"
    };

    public string GetTemplate(CsvImportType type) => type switch
    {
        CsvImportType.Products =>
            "sku;name;water_type_id;price_kopecks;returnable;capacity_units;active\n" +
            "NEW-001;Вода артезианская 19 л;1;27000;1;2;1\n",
        _ =>
            "warehouse_name;product_sku;quantity;comment\n" +
            "Центральный склад;WT-019;20;Поставка от поставщика\n"
    };

    public async Task<string> SaveTemplateAsync(CsvImportType type, string path, CancellationToken ct = default)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        await File.WriteAllTextAsync(path, GetTemplate(type), new System.Text.UTF8Encoding(true), ct);
        return path;
    }

    public async Task<CsvImportResult> ReadAndValidateAsync(string path, CsvImportType type, CancellationToken ct = default)
    {
        var result = new CsvImportResult { ImportType = type };
        var parsed = await _csv.ReadCsvAsync(path, ct);
        if (parsed.Count == 0)
        {
            result.Errors.Add(new CsvImportError(1, "Файл", "Файл пуст или не содержит корректных строк."));
            return result;
        }

        var headers = parsed[0].Select(h => h.Trim().TrimStart('\uFEFF')).ToArray();
        var expected = type == CsvImportType.Products
            ? new[] { "sku", "name", "water_type_id", "price_kopecks", "returnable", "capacity_units", "active" }
            : new[] { "warehouse_name", "product_sku", "quantity", "comment" };

        var missing = expected.Except(headers, StringComparer.OrdinalIgnoreCase).ToArray();
        if (missing.Length > 0)
        {
            result.Errors.Add(new CsvImportError(1, "Заголовки", $"Отсутствуют обязательные колонки: {string.Join(", ", missing)}"));
            return result;
        }

        var seenSkus = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var index = 1; index < parsed.Count; index++)
        {
            var line = index + 1;
            var values = expected.ToDictionary(
                key => key,
                key => GetValue(parsed[index], headers, key),
                StringComparer.OrdinalIgnoreCase);
            var errorsBefore = result.Errors.Count;

            if (type == CsvImportType.Products)
                ValidateProduct(values, line, seenSkus, result.Errors);
            else
                ValidateReceipt(values, line, result.Errors);

            var hasError = result.Errors.Count > errorsBefore;
            result.PreviewRows.Add(CreatePreview(type, values, line, result.Errors.Skip(errorsBefore).ToArray(), hasError));
            if (!hasError)
                result.ValidRows.Add(values);
        }

        if (result.ValidRows.Count == 0 && result.Errors.Count == 0)
            result.Errors.Add(new CsvImportError(1, "Файл", "Не найдено строк с данными после заголовков."));
        return result;
    }

    public Task WriteErrorsAsync(CsvImportResult result, string path, CancellationToken ct = default) =>
        _csv.WriteImportErrorsAsync(path, result.Errors, ct);

    private static void ValidateProduct(IReadOnlyDictionary<string, string> values, int line,
        HashSet<string> seenSkus, List<CsvImportError> errors)
    {
        var sku = values["sku"];
        if (string.IsNullOrWhiteSpace(sku)) AddError(errors, line, "sku", "Артикул обязателен.");
        else if (!seenSkus.Add(sku)) AddError(errors, line, "sku", $"Артикул «{sku}» повторяется в файле.");
        if (string.IsNullOrWhiteSpace(values["name"])) AddError(errors, line, "name", "Название товара обязательно.");
        if (!int.TryParse(values["water_type_id"], out var waterType) || waterType is < 1 or > 3)
            AddError(errors, line, "water_type_id", "Тип воды должен быть 1, 2 или 3.");
        if (!int.TryParse(values["price_kopecks"], out var price) || price < 0)
            AddError(errors, line, "price_kopecks", "Цена должна быть целым числом не меньше 0.");
        if (!int.TryParse(values["capacity_units"], out var capacity) || capacity <= 0)
            AddError(errors, line, "capacity_units", "Условные места должны быть больше 0.");
        ValidateFlag(values["returnable"], "returnable", line, errors);
        ValidateFlag(values["active"], "active", line, errors);
    }

    private static void ValidateReceipt(IReadOnlyDictionary<string, string> values, int line, List<CsvImportError> errors)
    {
        var warehouse = values["warehouse_name"];
        if (!Warehouses.Contains(warehouse))
            AddError(errors, line, "warehouse_name", $"Склад «{warehouse}» не найден. Доступные: {string.Join(", ", Warehouses)}.");
        var sku = values["product_sku"];
        if (!ExistingSkus.Contains(sku))
            AddError(errors, line, "product_sku", $"Товар «{sku}» не найден.");
        if (!int.TryParse(values["quantity"], out var quantity) || quantity <= 0)
            AddError(errors, line, "quantity", "Количество должно быть целым числом больше 0.");
    }

    private static void ValidateFlag(string value, string field, int line, List<CsvImportError> errors)
    {
        if (value != "0" && value != "1")
            AddError(errors, line, field, "Значение должно быть 0 или 1.");
    }

    private static void AddError(List<CsvImportError> errors, int line, string field, string message) =>
        errors.Add(new CsvImportError(line, field, message));

    private static CsvPreviewRow CreatePreview(CsvImportType type, IReadOnlyDictionary<string, string> values,
        int line, IReadOnlyCollection<CsvImportError> errors, bool hasError)
    {
        var result = hasError
            ? "Ошибка: " + string.Join(" ", errors.Select(e => $"{e.Field}: {e.Message}"))
            : "Готово";
        return type == CsvImportType.Products
            ? new CsvPreviewRow(line, values["name"], values["sku"], values["capacity_units"], result, hasError)
            : new CsvPreviewRow(line, values["warehouse_name"], values["product_sku"], values["quantity"], result, hasError);
    }

    private static string GetValue(string[] row, string[] headers, string key)
    {
        var index = Array.FindIndex(headers, h => h.Equals(key, StringComparison.OrdinalIgnoreCase));
        return index >= 0 && index < row.Length ? row[index].Trim() : string.Empty;
    }
}
