namespace WaterRoute.Forms;

public partial class RoutePreviewForm : BaseEditForm
{
    public RoutePreviewForm()
        : base("Маршрут R-021", "Просмотр направления доставки, статуса и временного окна", 1100, 760)
    {
        InitializeComponent();
    }

    private void offlineButton_Click(object? sender, EventArgs e)
    {
        using var form = new OfflineCallForm();
        form.ShowDialog(this);
    }
}
