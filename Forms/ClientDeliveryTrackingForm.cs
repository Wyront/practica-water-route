using WaterRoute.Controls;
using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

public sealed partial class ClientDeliveryTrackingForm : BaseEditForm
{
    public ClientDeliveryTrackingForm() : base("Моя доставка", "Отслеживание текущей доставки и просмотр маршрута курьера", 1100, 760)
    {
        InitializeComponent();
    }

    private void RefreshData()
    {
        // TODO: загрузка текущего маршрута клиента
    }

    private void openMapButton_Click(object? sender, EventArgs e)
    {
        using var form = new RoutePreviewForm();
        form.ShowDialog(this);
    }

    private void callDispatcherButton_Click(object? sender, EventArgs e)
    {
        using var form = new OfflineCallForm();
        form.ShowDialog(this);
    }
}