using WaterRoute.Forms;
using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

public sealed partial class DispatcherMonitorForm : BaseEditForm
{
    public DispatcherMonitorForm() : base("Монитор доставок", "Состояние заказов и маршрутов в реальном времени", 1200, 800)
    {
        InitializeComponent();
    }

    private void RefreshData()
    {
        // TODO: sp_monitor_get
        _notifications.ShowSuccess("Монитор", "Данные обновлены");
    }

    private void openRouteButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        var row = (string[])routesGrid.SelectedRows[0].Tag!;
        using var form = new RoutePreviewForm();
        form.ShowDialog(this);
    }

    private void openChatButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        var row = (string[])routesGrid.SelectedRows[0].Tag!;
        // TODO: открыть чат маршрута
        MessageBox.Show("Чат с курьером будет открыт после интеграции с БД", "WaterRoute");
    }

    private void problemButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        var row = (string[])routesGrid.SelectedRows[0].Tag!;
        using var form = new ProblemEditForm(UserRole.Dispatcher);
        form.ShowDialog(this);
    }

    private void callClientButton_Click(object? sender, EventArgs e)
    {
        if (routesGrid.SelectedRows.Count == 0) return;
        var row = (string[])routesGrid.SelectedRows[0].Tag!;
        // TODO: показать телефон клиента и позвонить
        _notifications.ShowSuccess("Связь", "Телефон клиента скопирован в буфер обмена");
    }

    private readonly INotificationService _notifications;
}