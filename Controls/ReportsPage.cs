using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Controls;

public sealed partial class ReportsPage : BasePage
{
    private readonly UserSession _session;
    private readonly ReportExportService _exportService = new();

    public ReportsPage() : this(RoleInfo.CreateDemoSession(UserRole.Admin))
    {
    }

    public ReportsPage(UserSession session) : base("Отчёты", "Данные формируются JOIN-запросами в хранимых процедурах и экспортируются одним файлом")
    {
        _session = session;
        InitializeComponent();
        reportTypeComboBox.Items.AddRange(GetReportTypes(session.Role));
        reportTypeComboBox.SelectedIndex = 0;
        LoadReport(reportTypeComboBox.SelectedItem?.ToString() ?? "Отчёт");
    }

    private void LoadReport(string reportType)
    {
        grid.Rows.Clear();
        grid.Rows.Add("Курьер Алексей", "8", "42", "4,9", "86 400 ₽");
        grid.Rows.Add("Курьер Сергей", "7", "36", "4,7", "72 800 ₽");
        grid.Rows.Add("Итого", "15", "78", "4,8", "159 200 ₽");
        summaryLabel.Text = $"{reportType}: 3 строки · сформирован {DateTime.Now:dd.MM.yyyy HH:mm}";
    }

    private static string[] GetReportTypes(UserRole role) => role switch
    {
        UserRole.Storekeeper => ["Складские остатки и движения", "Оборот и долг возвратной тары"],
        UserRole.Dispatcher => ["Заказы и выручка", "Выполнение маршрутов и недоставки", "Топ курьеров", "Зарегистрированные проблемы"],
        _ => ["Заказы и выручка", "Выполнение маршрутов и недоставки", "Топ курьеров", "Складские остатки и движения", "Оборот и долг возвратной тары", "Зарегистрированные проблемы"]
    };

    private void generateButton_Click(object? sender, EventArgs e) => LoadReport(reportTypeComboBox.SelectedItem?.ToString() ?? "Отчёт");
    private async void exportCsvButton_Click(object? sender, EventArgs e) => await ExportAsync("csv");
    private async void exportPdfButton_Click(object? sender, EventArgs e) => await ExportAsync("pdf");

    private async Task ExportAsync(string format)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = format == "csv" ? "CSV-файлы (*.csv)|*.csv" : "PDF-файлы (*.pdf)|*.pdf",
            FileName = $"waterroute-report-{DateTime.Now:yyyyMMdd-HHmmss}.{format}",
            Title = $"Экспорт отчёта в {format.ToUpperInvariant()}"
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            var headers = grid.Columns.Cast<DataGridViewColumn>().Select(c => c.HeaderText).ToArray();
            var rows = grid.Rows.Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .Select(r => r.Cells.Cast<DataGridViewCell>().Select(c => c.Value?.ToString() ?? string.Empty).ToArray())
                .ToArray();
            var title = reportTypeComboBox.SelectedItem?.ToString() ?? "Отчёт WaterRoute";
            var parameters = $"Период: {fromDatePicker.Value:dd.MM.yyyy} — {toDatePicker.Value:dd.MM.yyyy}";

            if (format == "csv")
                await _exportService.ExportCsvAsync(dialog.FileName, headers, rows);
            else
                await _exportService.ExportPdfAsync(dialog.FileName, title, headers, rows, parameters);

            MessageBox.Show($"Отчёт сохранён одним файлом:\n{dialog.FileName}", "Экспорт завершён",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка экспорта: {ex.Message}", "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
