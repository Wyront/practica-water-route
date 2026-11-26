using WaterRoute.Forms;
using WaterRoute.Models;

namespace WaterRoute.Controls;

public partial class MonitorPage : BasePage
{
    public MonitorPage() : base(
        "Монитор маршрутов",
        "Состояние заказов и маршрутов. Автоматическое обновление каждые 30 секунд")
    {
        InitializeComponent();
    }

    private void refreshButton_Click(object? sender, EventArgs e) => updatedLabel.Text = $"Обновлено: {DateTime.Now:HH:mm:ss}";

    private void openRouteButton_Click(object? sender, EventArgs e)
    {
        using var form = new RoutePreviewForm();
        form.ShowDialog(this);
    }

    private void problemButton_Click(object? sender, EventArgs e)
    {
        using var form = new ProblemEditForm(UserRole.Dispatcher);
        form.ShowDialog(this);
    }
}
