using WaterRoute.Forms;
using WaterRoute.Models;

namespace WaterRoute.Controls;

public partial class CalendarPage : BasePage
{
    public CalendarPage() : base(
        "Календарь доставок",
        "Месячный и недельный обзор заказов и маршрутов с цветовой маркировкой статусов")
    {
        InitializeComponent();
    }

    private void eventsGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        using var form = new OrderEditForm(UserRole.Dispatcher, true);
        form.ShowDialog(this);
    }
}
