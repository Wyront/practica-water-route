using System.Text;
using WaterRoute.Models;

namespace WaterRoute.Services;

public sealed class CsvFileService
{
    public const char Delimiter = ';';

    public async Task<IReadOnlyList<string[]>> ReadCsvAsync(string path, CancellationToken ct = default)
    {
        var text = await File.ReadAllTextAsync(path, Encoding.UTF8, ct);
        return Parse(text);
    }

    public async Task WriteCsvAsync(string path, IEnumerable<string> headers, IEnumerable<IEnumerable<string>> rows, CancellationToken ct = default)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        await using var writer = new StreamWriter(path, false, new UTF8Encoding(true));
        await writer.WriteLineAsync(string.Join(Delimiter, headers.Select(Escape)));
        foreach (var row in rows)
            await writer.WriteLineAsync(string.Join(Delimiter, row.Select(Escape)));
    }

    public async Task WriteImportErrorsAsync(string path, IEnumerable<CsvImportError> errors, CancellationToken ct = default)
    {
        await WriteCsvAsync(path,
            ["Строка", "Поле", "Ошибка"],
            errors.Select(e => (IEnumerable<string>)[$"Строка {e.LineNumber}", e.Field, e.Message]), ct);
    }

    private static string Escape(string? value)
    {
        value ??= string.Empty;
        if (value.Contains(Delimiter) || value.Contains('"') || value.Contains('\r') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    private static IReadOnlyList<string[]> Parse(string text)
    {
        var rows = new List<string[]>();
        var row = new List<string>();
        var field = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < text.Length; i++)
        {
            var ch = text[i];
            if (inQuotes)
            {
                if (ch == '"')
                {
                    if (i + 1 < text.Length && text[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    field.Append(ch);
                }
                continue;
            }

            if (ch == '"' && field.Length == 0)
            {
                inQuotes = true;
            }
            else if (ch == Delimiter)
            {
                row.Add(field.ToString());
                field.Clear();
            }
            else if (ch == '\r' || ch == '\n')
            {
                if (ch == '\r' && i + 1 < text.Length && text[i + 1] == '\n')
                    i++;
                AddRow(rows, row, field);
            }
            else
            {
                field.Append(ch);
            }
        }

        AddRow(rows, row, field);
        return rows;
    }

    private static void AddRow(List<string[]> rows, List<string> row, StringBuilder field)
    {
        row.Add(field.ToString());
        field.Clear();
        if (row.Any(value => !string.IsNullOrWhiteSpace(value)))
            rows.Add(row.ToArray());
        row.Clear();
    }
}
