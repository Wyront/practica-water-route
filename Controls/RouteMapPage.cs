using WaterRoute.Forms;
using WaterRoute.Models;

namespace WaterRoute.Controls;

public partial class RouteMapPage : BasePage
{
    public RouteMapPage() : base(
        "Карта доставки",
        "Санкт-Петербург · онлайн-карта OpenStreetMap · без GPS и дорожной оптимизации")
    {
        InitializeComponent();
    }

    public RouteMapPage(UserSession session) : base(
        "Карта доставки",
        $"{RoleInfo.GetName(session.Role)} · Санкт-Петербург · без GPS и дорожной оптимизации")
    {
        InitializeComponent();
    }

    private void offlineButton_Click(object? sender, EventArgs e)
    {
        using var form = new OfflineCallForm();
        form.ShowDialog(this);
    }
}
