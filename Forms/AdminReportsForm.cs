using Microsoft.Extensions.DependencyInjection;
using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Forms;

public sealed partial class AdminReportsForm : BaseEditForm
{
    private readonly IRepository _repo;
    private readonly IServiceProvider _services;
    private readonly INotificationService _notifications;
    private readonly ReportExportService _exportService = new();

    public AdminReportsForm(
        IRepository repo,
        IServiceProvider services,
        INotificationService notifications)
        : base("Административные отчёты", "Полный доступ ко всем отчётам системы", 1200, 800)
    {
        _repo = repo;
        _services = services;
        _notifications = notifications;
        InitializeComponent();
        Load += (_, _) => LoadReportTypes();
    }

    // Для визуального дизайнера
    public AdminReportsForm() : this(null!, null!, null!) { }

    private void LoadReportTypes()
    {
        reportTypeComboBox.Items.Clear();
        reportTypeComboBox.Items.AddRange(new[] {
            "Маршруты и доставки",
            "Заказы и выручка",
            "Складские движения",
            "Оборот тары",
            "Проблемы"
        });
        reportTypeComboBox.SelectedIndex = 0;
    }

    private async void generateButton_Click(object? sender, EventArgs e)
    {
        if (_repo == null) return;

        var type = reportTypeComboBox.SelectedItem?.ToString() ?? "";
        var dateFrom = fromDatePicker.Value.Date;
        var dateTo = toDatePicker.Value.Date.AddDays(1).AddSeconds(-1);

        try
        {
            grid.Rows.Clear();
            grid.Columns.Clear();

            switch (type)
            {
                case "Маршруты и доставки":
                    await LoadRoutesDeliveriesReport(dateFrom, dateTo);
                    break;
                case "Заказы и выручка":
                    await LoadOrdersRevenueReport(dateFrom, dateTo);
                    break;
                case "Складские движения":
                    await LoadStockMovementsReport(dateFrom, dateTo);
                    break;
                case "Оборот тары":
                    await LoadContainerTurnoverReport(dateFrom, dateTo);
                    break;
                case "Проблемы":
                    await LoadProblemsReport(dateFrom, dateTo);
                    break;
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка генерации отчёта", ex.Message);
        }
    }

    private async Task LoadRoutesDeliveriesReport(DateTime from, DateTime to)
    {
        var rows = await _repo.QueryAsync<dynamic>("sp_report_routes_deliveries", new
        {
            p_date_from = from,
            p_date_to = to,
            p_courier_id = (int?)null,
            p_dispatcher_id = (int?)null
        });

        grid.Columns.Add("RouteNumber", "№ Маршрута");
        grid.Columns.Add("RouteDate", "Дата");
        grid.Columns.Add("CourierName", "Курьер");
        grid.Columns.Add("DispatcherName", "Диспетчер");
        grid.Columns.Add("TotalStops", "Всего остановок");
        grid.Columns.Add("CompletedStops", "Выполнено");
        grid.Columns.Add("FailedStops", "Не выполнено");

        foreach (IDictionary<string, object?> r in rows)
        {
            grid.Rows.Add(
                r["route_number"],
                r["route_date"],
                r["courier_name"],
                r["dispatcher_name"],
                r["total_stops"],
                r["completed_stops"],
                r["failed_stops"]
            );
        }
    }

    private async Task LoadOrdersRevenueReport(DateTime from, DateTime to)
    {
        var rows = await _repo.QueryAsync<dynamic>("sp_report_orders_revenue", new
        {
            p_date_from = from,
            p_date_to = to,
            p_warehouse_id = (int?)null
        });

        grid.Columns.Add("Date", "Дата");
        grid.Columns.Add("OrdersCount", "Заказов");
        grid.Columns.Add("Revenue", "Выручка, ₽");

        foreach (IDictionary<string, object?> r in rows)
        {
            grid.Rows.Add(r["date"], r["orders_count"], r["revenue_rub"]);
        }
    }

    private async Task LoadStockMovementsReport(DateTime from, DateTime to)
    {
        var rows = await _repo.QueryAsync<dynamic>("sp_report_stock_movements", new
        {
            p_date_from = from,
            p_date_to = to,
            p_warehouse_id = (int?)null,
            p_type = (string?)null
        });

        grid.Columns.Add("Warehouse", "Склад");
        grid.Columns.Add("Product", "Товар");
        grid.Columns.Add("Type", "Тип");
        grid.Columns.Add("Quantity", "Кол-во");
        grid.Columns.Add("Date", "Дата");
        grid.Columns.Add("Comment", "Комментарий");

        foreach (IDictionary<string, object?> r in rows)
        {
            grid.Rows.Add(r["warehouse_name"], r["product_name"], r["kind"], r["quantity"], r["occurred_at"], r["comment"]);
        }
    }

    private async Task LoadContainerTurnoverReport(DateTime from, DateTime to)
    {
        var rows = await _repo.QueryAsync<dynamic>("sp_report_container_turnover", new
        {
            p_date_from = from,
            p_date_to = to,
            p_client_id = (int?)null
        });

        grid.Columns.Add("Warehouse", "Склад");
        grid.Columns.Add("Client", "Клиент");
        grid.Columns.Add("WaterType", "Тип воды");
        grid.Columns.Add("Deposit", "Залог, ₽");
        grid.Columns.Add("Type", "Тип");
        grid.Columns.Add("Quantity", "Кол-во");
        grid.Columns.Add("Status", "Статус");
        grid.Columns.Add("Date", "Дата");

        foreach (IDictionary<string, object?> r in rows)
        {
            var depositKopecks = Convert.ToInt32(r["deposit_kopecks"]);
            grid.Rows.Add(r["warehouse_name"], r["client_name"], r["water_type_name"], depositKopecks / 100m, r["kind"], r["quantity"], r["confirmation_status"], r["occurred_at"]);
        }
    }

    private async Task LoadProblemsReport(DateTime from, DateTime to)
    {
        var rows = await _repo.QueryAsync<dynamic>("sp_report_problems", new
        {
            p_date_from = from,
            p_date_to = to,
            p_status = (string?)null
        });

        grid.Columns.Add("Order", "Заказ");
        grid.Columns.Add("Client", "Клиент");
        grid.Columns.Add("Type", "Тип");
        grid.Columns.Add("Status", "Статус");
        grid.Columns.Add("Created", "Создана");
        grid.Columns.Add("Resolved", "Решена");
        grid.Columns.Add("Resolution", "Решение");

        foreach (IDictionary<string, object?> r in rows)
        {
            grid.Rows.Add(r["order_number"], r["client_name"], r["type"], r["status"], r["created_at"], r["resolved_at"], r["resolution_comment"]);
        }
    }

    private async void exportCsvButton_Click(object? sender, EventArgs e) => await ExportAsync("csv");
    private async void exportPdfButton_Click(object? sender, EventArgs e) => await ExportAsync("pdf");

    private async Task ExportAsync(string format)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = format == "csv" ? "CSV-файлы (*.csv)|*.csv" : "PDF-файлы (*.pdf)|*.pdf",
            FileName = $"waterroute-admin-report-{DateTime.Now:yyyyMMdd-HHmmss}.{format}",
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
            var title = reportTypeComboBox.SelectedItem?.ToString() ?? "Административный отчёт";
            var parameters = $"Период: {fromDatePicker.Value:dd.MM.yyyy} — {toDatePicker.Value:dd.MM.yyyy}";

            if (format == "csv")
                await _exportService.ExportCsvAsync(dialog.FileName, headers, rows);
            else
                await _exportService.ExportPdfAsync(dialog.FileName, title, headers, rows, parameters);

            _notifications.ShowSuccess("Экспорт завершён", $"Отчёт сохранён:\n{dialog.FileName}");
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка экспорта", ex.Message);
        }
    }
}