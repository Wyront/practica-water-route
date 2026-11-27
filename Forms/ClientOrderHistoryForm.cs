using Microsoft.Extensions.DependencyInjection;
using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Forms;

public sealed partial class ClientOrderHistoryForm : BaseEditForm
{
    private readonly IRepository _repo;
    private readonly IServiceProvider _services;
    private readonly INotificationService _notifications;
    private int _clientId = -1;

    public ClientOrderHistoryForm(
        IRepository repo,
        IServiceProvider services,
        INotificationService notifications)
        : base("Мои заказы", "История заказов, статусы и повторные заказы", 1100, 700)
    {
        _repo = repo;
        _services = services;
        _notifications = notifications;
        InitializeComponent();
        Load += (_, _) => LoadHistory();
    }

    // Для визуального дизайнера
    public ClientOrderHistoryForm() : this(null!, null!, null!) { }

    public void LoadHistory(int clientId)
    {
        _clientId = clientId;
        LoadHistory();
    }

    public void RefreshData() => LoadHistory();

    private async void LoadHistory()
    {
        if (_repo == null || _clientId <= 0) return;

        try
        {
            var rows = await _repo.QueryAsync<dynamic>("sp_order_list", new 
            { 
                p_client_id = _clientId,
                p_status = (string?)null,
                p_courier_id = (int?)null,
                p_dispatcher_id = (int?)null,
                p_warehouse_id = (int?)null,
                p_date_from = (DateTime?)null,
                p_date_to = (DateTime?)null,
                p_search = (string?)null,
                p_limit = 1000,
                p_offset = 0
            });
            
            ordersGrid.Rows.Clear();
            foreach (IDictionary<string, object?> r in rows)
            {
                var index = ordersGrid.Rows.Add(
                    r["id"],
                    r["order_number"],
                    r["delivery_date"],
                    GetStatusDisplay(r["status"]?.ToString()),
                    r["total_price_kopecks"] != null ? (Convert.ToInt32(r["total_price_kopecks"]) / 100m).ToString("N2") + " ₽" : "—",
                    r["warehouse_name"],
                    r["delivery_address_snapshot"]
                );
                ordersGrid.Rows[index].Tag = r;
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка загрузки истории заказов", ex.Message);
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

    private void repeatOrderButton_Click(object? sender, EventArgs e)
    {
        if (ordersGrid.SelectedRows.Count == 0)
        {
            _notifications.ShowError("Ошибка", "Выберите заказ для повтора");
            return;
        }
        var row = (IDictionary<string, object?>)ordersGrid.SelectedRows[0].Tag!;
        var orderId = Convert.ToInt32(row["id"]);
        var status = row["status"]?.ToString();
        
        if (status != "delivered" && status != "not_delivered")
        {
            _notifications.ShowError("Ошибка", "Повторить можно только доставленные или недоставленные заказы");
            return;
        }
        
        if (MessageBox.Show("Повторить заказ? Будет создан новый заказ с теми же параметрами.", "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            RepeatOrder(orderId);
        }
    }

    private async void RepeatOrder(int orderId)
    {
        if (_repo == null || _clientId <= 0) return;
        
        try
        {
            await _repo.ExecuteAsync("sp_order_repeat", new { p_order_id = orderId, p_client_id = _clientId });
            _notifications.ShowSuccess("Повтор заказа", "Заказ успешно повторён");
            LoadHistory();
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка", ex.Message);
        }
    }

    private void cancelOrderButton_Click(object? sender, EventArgs e)
    {
        if (ordersGrid.SelectedRows.Count == 0) return;
        var row = (IDictionary<string, object?>)ordersGrid.SelectedRows[0].Tag!;
        var orderId = Convert.ToInt32(row["id"]);
        var status = row["status"]?.ToString();
        
        if (status != "new")
        {
            _notifications.ShowError("Ошибка", "Отменить можно только заказ в статусе «Новый\". Для других статусов обратитесь к диспетчеру.");
            return;
        }
        if (MessageBox.Show("Отменить выбранный заказ?", "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            CancelOrder(orderId);
        }
    }

    private async void CancelOrder(int orderId)
    {
        if (_repo == null) return;
        
        try
        {
            // We need a user ID - for client it would be their own ID
            await _repo.ExecuteAsync("sp_order_cancel", new { p_order_id = orderId, p_user_id = _clientId, p_reason = "Отменён клиентом" });
            _notifications.ShowSuccess("Заказ отменён", "Заказ успешно отменён");
            LoadHistory();
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка", ex.Message);
        }
    }

    private void viewOrderButton_Click(object? sender, EventArgs e)
    {
        if (ordersGrid.SelectedRows.Count == 0) return;
        var row = (IDictionary<string, object?>)ordersGrid.SelectedRows[0].Tag!;
        var orderId = Convert.ToInt32(row["id"]);
        using var form = _services.GetRequiredService<OrderEditForm>();
        form.LoadOrder(orderId);
        form.ShowDialog(this);
    }

    private void pdfButton_Click(object? sender, EventArgs e)
    {
        if (ordersGrid.SelectedRows.Count == 0) return;
        _notifications.ShowError("Ошибка", "Генерация PDF в разработке");
    }
}