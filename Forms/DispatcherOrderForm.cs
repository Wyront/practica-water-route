using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

public sealed partial class DispatcherOrderForm : BaseEditForm
{
    public DispatcherOrderForm() : base("Управление заказами", "Очередь распределения, создание и редактирование заказов, контроль ресурсов", 1200, 750)
    {
        InitializeComponent();
    }

    private void RefreshData()
    {
        // TODO: sp_order_list с фильтрами для диспетчера
    }

    private void createButton_Click(object? sender, EventArgs e)
    {
        using var form = new OrderEditForm(UserRole.Dispatcher);
        if (form.ShowDialog(this) == DialogResult.OK)
            RefreshData();
    }

    private void editButton_Click(object? sender, EventArgs e)
    {
        if (ordersGrid.SelectedRows.Count == 0) return;
        var row = (string[])ordersGrid.SelectedRows[0].Tag!;
        using var form = new OrderEditForm(UserRole.Dispatcher);
        form.ShowDialog(this);
        RefreshData();
    }

    private void changeWarehouseButton_Click(object? sender, EventArgs e)
    {
        if (ordersGrid.SelectedRows.Count == 0) return;
        var row = (string[])ordersGrid.SelectedRows[0].Tag!;
        var status = row[5];
        if (status != "Новый" && status != "Запланирован")
        {
            MessageBox.Show("Менять склад можно только для заказов со статусом «Новый» или «Запланированный»", "WaterRoute");
            return;
        }
        // TODO: sp_order_change_warehouse
        MessageBox.Show("Смена склада будет выполнена после интеграции с БД", "WaterRoute");
    }

    private void reassignButton_Click(object? sender, EventArgs e)
    {
        if (ordersGrid.SelectedRows.Count == 0) return;
        var row = (string[])ordersGrid.SelectedRows[0].Tag!;
        var status = row[5];
        if (status != "Запланирован")
        {
            MessageBox.Show("Изменять назначение можно только для запланированных заказов", "WaterRoute");
            return;
        }
        // TODO: открыть форму переназначения
        MessageBox.Show("Переназначение будет подключено после интеграции с БД", "WaterRoute");
    }

    private void cancelButton_Click(object? sender, EventArgs e)
    {
        if (ordersGrid.SelectedRows.Count == 0) return;
        var row = (string[])ordersGrid.SelectedRows[0].Tag!;
        var status = row[5];
        if (status != "Новый" && status != "Запланирован")
        {
            MessageBox.Show("Отменить можно только заказы в статусе «Новый» или «Запланированный»", "WaterRoute");
            return;
        }
        if (MessageBox.Show("Отменить выбранный заказ? Резерв товаров будет освобождён.", "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            // TODO: sp_order_cancel
            MessageBox.Show("Заказ будет отменён после интеграции с БД", "WaterRoute");
        }
    }

    private void processQueueButton_Click(object? sender, EventArgs e)
    {
        // TODO: sp_auto_assign_process_queue
        _notifications.ShowSuccess("Очередь", "Обработка очереди запущена. Новые заказы будут распределены по мере доступности ресурсов.");
    }

    private readonly INotificationService _notifications;
}