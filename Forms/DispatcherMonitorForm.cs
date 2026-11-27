using Microsoft.Extensions.DependencyInjection;
using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Forms;

public sealed partial class DispatcherMonitorForm : BaseEditForm
{
    private readonly IRepository _repo;
    private readonly IServiceProvider _services;
    private readonly INotificationService _notifications;
    private CancellationTokenSource? _refreshCts;

    public DispatcherMonitorForm(
        IRepository repo,
        IServiceProvider services,
        INotificationService notifications)
        : base("Монитор доставок", "Состояние заказов и маршрутов в реальном времени", 1200, 800)
    {
        _repo = repo;
        _services = services;
        _notifications = notifications;
        InitializeComponent();
        Load += (_, _) => RefreshData();
        FormClosed += (_, _) => _refreshCts?.Cancel();
    }

    // Для визуального дизайнера
    public DispatcherMonitorForm() : this(null!, null!, null!) { }

    private async void RefreshData()
    {
        if (_repo == null) return;
        
        try
        {
            var today = DateTime.Today;
            var rows = await _repo.QueryAsync<dynamic>("sp_monitor_get", new { p_date = today });
            
            routesGrid.Rows.Clear();
            foreach (IDictionary<string, object?> r in rows)
            {
                var index = routesGrid.Rows.Add(
                    r["route_number"],
                    r["order_number"],
                    r["courier_name"],
                    r["vehicle_plate"],
                    GetStatusDisplay(r["status"]?.ToString()),
                    r["last_event"]?.ToString() ?? "—"
                );
                routesGrid.Rows[index].Tag = r;
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка загрузки монитора", ex.Message);
        }
    }

    private static string GetStatusDisplay(string? status) => status switch
    {
        "planned" => "Запланирован",
        "in_progress" => "В работе",
        "completed" => "Завершён",
        "cancelled" => "Отменён",
        _ => status ?? ""
    };

    private void openRouteButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        _notifications.ShowToast("Просмотр", "Функция просмотра маршрута в разработке", ToastType.Info);
    }

    private void openChatButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        _notifications.ShowToast("Чат", "Функция чата в разработке", ToastType.Info);
    }

    private void problemButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        _notifications.ShowToast("Проблема", "Функция фиксации проблемы в разработке", ToastType.Info);
    }

    private void callClientButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        var row = (IDictionary<string, object?>)routesGrid.SelectedRows[0].Tag!;
        var phone = row["client_phone"]?.ToString() ?? "";
        if (!string.IsNullOrEmpty(phone))
        {
            Clipboard.SetText(phone);
            _notifications.ShowSuccess("Связь", $"Телефон {phone} скопирован в буфер обмена");
        }
    }
}