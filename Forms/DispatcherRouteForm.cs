using Microsoft.Extensions.DependencyInjection;
using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Forms;

public sealed partial class DispatcherRouteForm : BaseEditForm
{
    private readonly IRepository _repo;
    private readonly IServiceProvider _services;
    private readonly INotificationService _notifications;

    public DispatcherRouteForm(
        IRepository repo,
        IServiceProvider services,
        INotificationService notifications)
        : base("Планировщик маршрутов", "Один заказ — один маршрут. Назначение курьера, автомобиля, диспетчера и временного окна", 1100, 700)
    {
        _repo = repo;
        _services = services;
        _notifications = notifications;
        InitializeComponent();
        Load += (_, _) => RefreshData();
    }

    // Для визуального дизайнера
    public DispatcherRouteForm() : this(null!, null!, null!) { }

    private async void RefreshData()
    {
        if (_repo == null) return;
        
        try
        {
            var rows = await _repo.QueryAsync<dynamic>("sp_route_list", new 
            { 
                p_status = (string?)null,
                p_courier_id = (int?)null,
                p_dispatcher_id = (int?)null,
                p_date_from = (DateTime?)null,
                p_date_to = (DateTime?)null,
                p_limit = 1000,
                p_offset = 0
            });
            
            routesGrid.Rows.Clear();
            foreach (IDictionary<string, object?> r in rows)
            {
                var index = routesGrid.Rows.Add(
                    r["id"],
                    r["route_number"],
                    r["route_date"],
                    GetStatusDisplay(r["status"]?.ToString()),
                    r["courier_name"]?.ToString() ?? "—",
                    r["dispatcher_name"]?.ToString() ?? "—",
                    r["total_stops"],
                    r["completed_stops"]
                );
                routesGrid.Rows[index].Tag = r;
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка загрузки маршрутов", ex.Message);
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

    private void editButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        _notifications.ShowToast("Редактирование", "Функция редактирования маршрута в разработке", ToastType.Info);
    }

    private void viewButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        var row = (IDictionary<string, object?>)routesGrid.SelectedRows[0].Tag!;
        var routeId = Convert.ToInt32(row["id"]);
        using var form = _services.GetRequiredService<RoutePreviewForm>();
        form.LoadRoute(routeId);
        form.ShowDialog(this);
    }

    private async void cancelRouteButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        var row = (IDictionary<string, object?>)routesGrid.SelectedRows[0].Tag!;
        var routeId = Convert.ToInt32(row["id"]);
        var status = row["status"]?.ToString();
        if (status == "completed")
        {
            MessageBox.Show("Завершённый маршрут нельзя отменить", "WaterRoute");
            return;
        }
        if (MessageBox.Show("Отменить маршрут? Заказ вернётся в очередь, ресурсы освободятся.", "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            try
            {
                await _repo.ExecuteAsync("sp_route_cancel", new { p_route_id = routeId, p_reason = "Отменён диспетчером" });
                _notifications.ShowSuccess("Маршрут отменён", "Маршрут успешно отменён, ресурсы освобождены");
                RefreshData();
            }
            catch (Exception ex)
            {
                _notifications.ShowError("Ошибка", ex.Message);
            }
        }
    }

    private async void requeueButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        var row = (IDictionary<string, object?>)routesGrid.SelectedRows[0].Tag!;
        var routeId = Convert.ToInt32(row["id"]);
        var status = row["status"]?.ToString();
        if (status != "not_delivered")
        {
            MessageBox.Show("Вернуть в очередь можно только недоставленные маршруты", "WaterRoute");
            return;
        }
        try
        {
            await _repo.ExecuteAsync("sp_route_cancel_resources", new { p_route_id = routeId });
            _notifications.ShowSuccess("В очередь", "Маршрут отменён, заказы возвращены в очередь");
            RefreshData();
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка", ex.Message);
        }
    }
}