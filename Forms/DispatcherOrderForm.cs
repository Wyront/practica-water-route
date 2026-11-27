using Microsoft.Extensions.DependencyInjection;
using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Forms;

public sealed partial class DispatcherOrderForm : BaseEditForm
{
    private readonly IRepository _repo;
    private readonly IServiceProvider _services;
    private readonly INotificationService _notifications;
    private readonly ICurrentUser _currentUser;

    public DispatcherOrderForm(
        IRepository repo,
        IServiceProvider services,
        ICurrentUser currentUser,
        INotificationService notifications)
        : base("Управление заказами", "Очередь распределения, создание и редактирование заказов, контроль ресурсов", 1200, 750)
    {
        _repo = repo;
        _services = services;
        _currentUser = currentUser;
        _notifications = notifications;
        InitializeComponent();
        Load += (_, _) => RefreshData();
    }

    // Для визуального дизайнера
    public DispatcherOrderForm() : this(null!, null!, null!, null!) { }

    private async void RefreshData()
    {
        if (_repo == null) return;
        
        try
        {
            var statusFilter = this.statusFilter.SelectedItem?.ToString();
            var search = string.IsNullOrWhiteSpace(searchTextBox.Text) ? null : searchTextBox.Text;
            
            var rows = await _repo.QueryAsync<dynamic>("sp_order_list", new 
            { 
                p_status = statusFilter == "Все" ? null : statusFilter,
                p_client_id = (int?)null,
                p_courier_id = (int?)null,
                p_dispatcher_id = (int?)null,
                p_warehouse_id = (int?)null,
                p_date_from = (DateTime?)null,
                p_date_to = (DateTime?)null,
                p_search = search,
                p_limit = 1000,
                p_offset = 0
            });
            
            ordersGrid.Rows.Clear();
            foreach (IDictionary<string, object?> r in rows)
            {
                var index = ordersGrid.Rows.Add(
                    r["id"],
                    r["order_number"],
                    r["client_name"],
                    r["delivery_address_snapshot"],
                    r["warehouse_name"],
                    GetStatusDisplay(r["status"]?.ToString()),
                    $"{r["delivery_time_start"]}–{r["delivery_time_end"]}",
                    r["courier_name"]?.ToString() ?? "—"
                );
                ordersGrid.Rows[index].Tag = r;
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка загрузки заказов", ex.Message);
        }
    }

    private static string GetStatusDisplay(string? status) => status switch
    {
        "new" => "Новый",
        "planned" => "Запланирован",
        "in_delivery" => "В доставке",
        "delivered" => "Доставлен",
        "not_delivered" => "Не доставлен",
        "cancelled" => "Отменён",
        _ => status ?? ""
    };

    private void createButton_Click(object? sender, EventArgs e)
    {
        _notifications.ShowToast("Создание заказа", "Функция в разработке", ToastType.Info);
    }

    private void editButton_Click(object? sender, EventArgs e)
    {
        if (ordersGrid.SelectedRows.Count == 0) return;
        _notifications.ShowToast("Редактирование", "Функция редактирования заказа в разработке", ToastType.Info);
    }

    private async void changeWarehouseButton_Click(object? sender, EventArgs e)
    {
        if (ordersGrid.SelectedRows.Count == 0) return;
        var row = (IDictionary<string, object?>)ordersGrid.SelectedRows[0].Tag!;
        var orderId = Convert.ToInt32(row["id"]);
        var status = row["status"]?.ToString();
        if (status != "new" && status != "planned")
        {
            MessageBox.Show("Менять склад можно только для заказов со статусом «Новый» или «Запланирован»", "WaterRoute");
            return;
        }

        using var form = new WarehouseSelectForm(_repo);
        if (form.ShowDialog(this) == DialogResult.OK && form.SelectedWarehouseId.HasValue)
        {
            try
            {
                await _repo.ExecuteAsync("sp_order_change_warehouse", new { p_order_id = orderId, p_new_warehouse_id = form.SelectedWarehouseId.Value });
                _notifications.ShowSuccess("Склад изменён", "Заказ успешно перемещён на другой склад");
                RefreshData();
            }
            catch (Exception ex)
            {
                _notifications.ShowError("Ошибка", ex.Message);
            }
        }
    }

    private void reassignButton_Click(object? sender, EventArgs e)
    {
        if (ordersGrid.SelectedRows.Count == 0) return;
        _notifications.ShowToast("Переназначение", "Функция в разработке", ToastType.Info);
    }

    private async void cancelButton_Click(object? sender, EventArgs e)
    {
        if (ordersGrid.SelectedRows.Count == 0) return;
        var row = (IDictionary<string, object?>)ordersGrid.SelectedRows[0].Tag!;
        var orderId = Convert.ToInt32(row["id"]);
        var status = row["status"]?.ToString();
        if (status != "new" && status != "planned")
        {
            MessageBox.Show("Отменить можно только заказы в статусе «Новый» или «Запланирован»", "WaterRoute");
            return;
        }
        if (MessageBox.Show("Отменить выбранный заказ? Резерв товаров будет освобождён.", "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            try
            {
                var session = _currentUser.Session;
                if (session != null)
                {
                    await _repo.ExecuteAsync("sp_order_cancel", new { p_order_id = orderId, p_user_id = session.Id, p_reason = "Отменён диспетчером" });
                    _notifications.ShowSuccess("Заказ отменён", "Заказ успешно отменён, резервы освобождены");
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                _notifications.ShowError("Ошибка", ex.Message);
            }
        }
    }

    private async void processQueueButton_Click(object? sender, EventArgs e)
    {
        try
        {
            await _repo.ExecuteAsync("sp_auto_assign_process_queue", null);
            _notifications.ShowSuccess("Очередь", "Обработка очереди запущена. Новые заказы будут распределены по мере доступности ресурсов.");
            RefreshData();
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка", ex.Message);
        }
    }
}