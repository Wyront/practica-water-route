using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

public sealed partial class DispatcherRouteForm : BaseEditForm
{
    public DispatcherRouteForm() : base("Планировщик маршрутов", "Один заказ — один маршрут. Назначение курьера, автомобиля, диспетчера и временного окна", 1100, 700)
    {
        InitializeComponent();
    }

    private void RefreshData()
    {
        // TODO: sp_route_list
    }

    private void editButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        var row = (string[])routesGrid.SelectedRows[0].Tag!;
        var status = row[7];
        if (status != "Запланирован")
        {
            MessageBox.Show("Изменять назначение можно только для маршрутов со статусом «Запланирован»", "WaterRoute");
            return;
        }
        using var form = new RouteEditForm();
        if (form.ShowDialog(this) == DialogResult.OK)
            RefreshData();
    }

    private void viewButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        var row = (string[])routesGrid.SelectedRows[0].Tag!;
        using var form = new RoutePreviewForm();
        form.ShowDialog(this);
    }

    private void cancelRouteButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        var row = (string[])routesGrid.SelectedRows[0].Tag!;
        var status = row[7];
        if (status == "Завершён")
        {
            MessageBox.Show("Завершённый маршрут нельзя отменить", "WaterRoute");
            return;
        }
        if (MessageBox.Show("Отменить маршрут? Заказ вернётся в очередь, ресурсы освободятся.", "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            // TODO: sp_route_cancel
            MessageBox.Show("Маршрут будет отменён после интеграции с БД", "WaterRoute");
        }
    }

    private void requeueButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        var row = (string[])routesGrid.SelectedRows[0].Tag!;
        var status = row[7];
        if (status != "Не доставлен")
        {
            MessageBox.Show("Вернуть в очередь можно только недоставленные маршруты", "WaterRoute");
            return;
        }
        // TODO: sp_order_requeue_after_fail
        MessageBox.Show("Заказ будет возвращён в очередь после интеграции с БД", "WaterRoute");
    }
}