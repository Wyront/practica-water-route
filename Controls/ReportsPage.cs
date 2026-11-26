using WaterRoute.Models;

namespace WaterRoute.Controls;

public sealed partial class ReportsPage : BasePage
{
    private readonly UserSession _session;

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

    private void exportCsvButton_Click(object? sender, EventArgs e) => ShowExportMessage("CSV");
    private void exportPdfButton_Click(object? sender, EventArgs e) => ShowExportMessage("PDF");

    private static void ShowExportMessage(string format) => MessageBox.Show(
        $"Экспорт {format} будет подключён после реализации процедуры получения отчёта и соответствующего файлового сервиса.\nПечать не используется.",
        $"Экспорт {format}", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
