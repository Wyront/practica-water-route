using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

public sealed partial class CourierRoutesForm : BaseEditForm
{
    public CourierRoutesForm() : base("Мои маршруты", "Список назначенных маршрутов, история и текущий статус", 1100, 700)
    {
        InitializeComponent();
    }

    private void RefreshData()
    {
        // TODO: sp_route_list с фильтром по courier_id
    }

    private void openRouteButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        var row = (string[])routesGrid.SelectedRows[0].Tag!;
        using var form = new RoutePreviewForm();
        form.ShowDialog(this);
    }

    private void startRouteButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        var row = (string[])routesGrid.SelectedRows[0].Tag!;
        var status = row[7];
        if (status != "Запланирован")
        {
            MessageBox.Show("Начать можно только запланированный маршрут", "WaterRoute");
            return;
        }
        // TODO: sp_route_start
        _notifications.ShowSuccess("Маршрут", "Маршрут начат. Статус изменён на «В работе»");
    }

    private void openChatButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        var row = (string[])routesGrid.SelectedRows[0].Tag!;
        // TODO: открыть чат маршрута
        MessageBox.Show("Чат с диспетчером будет открыт после интеграции с БД", "WaterRoute");
    }

    private void historyButton_Click(object? sender, EventArgs e)
    {
        using var form = new CourierHistoryForm();
        form.ShowDialog(this);
    }

    private readonly INotificationService _notifications;
}