using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Controls;

public sealed partial class NotificationsPage : BasePage
{
    public NotificationsPage() : base("Уведомления", "Центр уведомлений — важные события системы")
    {
        InitializeComponent();
    }

    public async Task LoadNotificationsAsync()
    {
        // TODO: загрузка через sp_notification_list
        await Task.CompletedTask;
    }

    private void markAllReadButton_Click(object? sender, EventArgs e)
    {
        // TODO: sp_notification_mark_all_read
        MessageBox.Show("Функция будет подключена после интеграции с БД", "WaterRoute");
    }

    private void grid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && _grid.Rows[e.RowIndex].Tag is string[] row)
        {
            var notificationId = row[0];
            // TODO: переход к связанной сущности
            MessageBox.Show($"Открыть уведомление {notificationId}", "WaterRoute");
        }
    }
}