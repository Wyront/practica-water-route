using Microsoft.Extensions.DependencyInjection;
using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Forms;

public partial class OrderEditForm : BaseEditForm
{
    private readonly IRepository _repo;
    private readonly IServiceProvider _services;
    private readonly INotificationService _notifications;
    private int _orderId = -1;

    public OrderEditForm() : this(UserRole.Dispatcher) { }

    public OrderEditForm(UserRole role, bool readOnly = false)
        : base("Новый заказ", "Один заказ обслуживается одним складом. Остатки проверяются перед созданием резерва.", 1040, 800)
    {
        InitializeComponent();
        SaveButton.Text = role == UserRole.Client ? "Оформить заказ" : "Сохранить заказ";
        SaveButton.Visible = !readOnly;
        addItemButton.Enabled = !readOnly;
        removeItemButton.Enabled = !readOnly;
    }

    public OrderEditForm(
        IRepository repo,
        IServiceProvider services,
        INotificationService notifications)
        : base("Новый заказ", "Один заказ обслуживается одним складом. Остатки проверяются перед созданием резерва.", 1040, 800)
    {
        _repo = repo;
        _services = services;
        _notifications = notifications;
        InitializeComponent();
    }

    public void InitializeForDispatcher()
    {
        SaveButton.Text = "Сохранить заказ";
        SaveButton.Visible = true;
        addItemButton.Enabled = true;
        removeItemButton.Enabled = true;
    }

    public async void LoadOrder(int orderId)
    {
        _orderId = orderId;
        await LoadOrderData();
    }

    private async Task LoadOrderData()
    {
        if (_repo == null || _orderId <= 0) return;

        try
        {
            var rows = await _repo.QueryAsync<dynamic>("sp_order_get", new { p_order_id = _orderId });
            var order = rows.FirstOrDefault();
            if (order == null) return;

            // Populate form fields from order data
            // This would need actual form controls - skipping for now
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка загрузки заказа", ex.Message);
        }
    }

    private void addItemButton_Click(object? sender, EventArgs e) => MessageBox.Show(
        "Добавление товара будет реализовано после подключения каталога и процедур БД.",
        "Добавление товара", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void removeItemButton_Click(object? sender, EventArgs e) => MessageBox.Show(
        "Удаление позиции будет реализовано после подключения процедур БД.",
        "Удаление позиции", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void checkStockButton_Click(object? sender, EventArgs e) => MessageBox.Show(
        "После подключения процедур здесь будет выполнена проверка физического остатка и резерва.",
        "Проверка остатков", MessageBoxButtons.OK, MessageBoxIcon.Information);
}