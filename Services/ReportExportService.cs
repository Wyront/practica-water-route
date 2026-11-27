using System.Drawing.Printing;

namespace WaterRoute.Services;

public sealed class ReportExportService
{
    private readonly CsvFileService _csv = new();

    public Task ExportCsvAsync(string path, IReadOnlyList<string> headers,
        IReadOnlyList<string[]> rows, CancellationToken ct = default) =>
        _csv.WriteCsvAsync(path, headers, rows, ct);

    public Task ExportPdfAsync(string path, string title, IReadOnlyList<string> headers,
        IReadOnlyList<string[]> rows, string? parameters = null, CancellationToken ct = default)
    {
        const string printerName = "Microsoft Print to PDF";
        if (!PrinterSettings.InstalledPrinters.Cast<string>().Contains(printerName, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Принтер «{printerName}» не найден. Установите компонент Windows «Microsoft Print to PDF».");

        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        using var document = new PrintDocument();
        document.DocumentName = title;
        document.PrinterSettings.PrinterName = printerName;
        document.PrinterSettings.PrintToFile = true;
        document.PrinterSettings.PrintFileName = path;
        document.PrintController = new StandardPrintController();

        var rowIndex = 0;
        var pageNumber = 0;
        document.BeginPrint += (_, _) =>
        {
            rowIndex = 0;
            pageNumber = 0;
        };
        document.PrintPage += (_, e) =>
        {
            pageNumber++;
            var g = e.Graphics;
            var bounds = e.MarginBounds;
            var columnWidth = bounds.Width / Math.Max(1, headers.Count);
            const int rowHeight = 30;

            using var titleFont = new Font("Segoe UI", 16, FontStyle.Bold);
            using var metaFont = new Font("Segoe UI", 8);
            using var headerFont = new Font("Segoe UI", 9, FontStyle.Bold);
            using var cellFont = new Font("Segoe UI", 8.5f);
            using var textBrush = new SolidBrush(Color.FromArgb(30, 41, 59));
            using var mutedBrush = new SolidBrush(Color.FromArgb(100, 116, 139));
            using var borderPen = new Pen(Color.FromArgb(203, 213, 225), 0.8f);
            using var headerBrush = new SolidBrush(Color.FromArgb(231, 239, 253));

            g.DrawString(title, titleFont, textBrush, bounds.Left, bounds.Top);
            var y = bounds.Top + 30;
            g.DrawString($"Сформировано: {DateTime.Now:dd.MM.yyyy HH:mm}", metaFont, mutedBrush, bounds.Left, y);
            y += 16;
            if (!string.IsNullOrWhiteSpace(parameters))
            {
                g.DrawString(parameters, metaFont, mutedBrush, bounds.Left, y);
                y += 16;
            }
            y += 18;

            for (var column = 0; column < headers.Count; column++)
            {
                var rect = new RectangleF(bounds.Left + column * columnWidth, y, columnWidth, rowHeight);
                g.FillRectangle(headerBrush, rect);
                g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width, rect.Height);
                g.DrawString(headers[column], headerFont, textBrush, rect);
            }
            y += rowHeight;

            while (rowIndex < rows.Count)
            {
                if (y + rowHeight > bounds.Bottom - 28)
                {
                    e.HasMorePages = true;
                    return;
                }

                var row = rows[rowIndex];
                for (var column = 0; column < headers.Count; column++)
                {
                    var value = column < row.Length ? row[column] : string.Empty;
                    var rect = new RectangleF(bounds.Left + column * columnWidth, y, columnWidth, rowHeight);
                    g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width, rect.Height);
                    g.DrawString(value, cellFont, textBrush, rect);
                }
                y += rowHeight;
                rowIndex++;
            }

            g.DrawString($"Страница {pageNumber}", metaFont, mutedBrush, bounds.Left, bounds.Bottom - 16);
            e.HasMorePages = false;
        };

        document.Print();
        return Task.CompletedTask;
    }
}
