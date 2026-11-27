using Microsoft.Extensions.DependencyInjection;
using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Forms;

public partial class RoutePreviewForm : BaseEditForm
{
    private readonly IRepository _repo;
    private readonly INotificationService _notifications;
    private int _routeId = -1;

    public RoutePreviewForm()
        : base("Маршрут", "Просмотр направления доставки, статуса и временного окна", 1100, 760)
    {
        InitializeComponent();
    }

    public RoutePreviewForm(
        IRepository repo,
        INotificationService notifications)
        : base("Маршрут", "Просмотр направления доставки, статуса и временного окна", 1100, 760)
    {
        _repo = repo;
        _notifications = notifications;
        InitializeComponent();
    }

    public async void LoadRoute(int routeId)
    {
        _routeId = routeId;
        await LoadRouteData();
    }

    private async Task LoadRouteData()
    {
        if (_repo == null || _routeId <= 0) return;

        try
        {
            var rows = await _repo.QueryAsync<dynamic>("sp_route_get", new { p_route_id = _routeId });
            var route = rows.FirstOrDefault();
            if (route == null)
            {
                _notifications.ShowError("Ошибка", "Маршрут не найден");
                return;
            }

            // Update form title and info panel
            Text = $"Маршрут {route["route_number"]}";
            titleInfoLabel.Text = $"{route["route_number"]} · {route["courier_name"]} · {GetStatusDisplay(route["status"]?.ToString())}";
            detailsLabel.Text = $"{route["courier_name"]} · {route["vehicle_plate"]} · {route["route_date"]:dd.MM.yyyy} · {route["warehouse_name"]}";

            // Load stops for this route
            await LoadStops();
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка загрузки маршрута", ex.Message);
        }
    }

    private async Task LoadStops()
    {
        if (_repo == null || _routeId <= 0) return;

        try
        {
            var rows = await _repo.QueryAsync<dynamic>("sp_route_get", new { p_route_id = _routeId });
            // The sp_route_get returns route with stops in the result
            // We'll need to extract stops from the result
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка загрузки остановок", ex.Message);
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

    private void offlineButton_Click(object? sender, EventArgs e)
    {
        using var form = new OfflineCallForm();
        form.ShowDialog(this);
    }
}