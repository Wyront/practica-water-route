using WaterRoute.Controls;
using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class ClientDeliveryTrackingForm
{
    private Panel infoPanel = null!;
    private DataGridView itemsGrid = null!;
    private RouteMapControl mapControl = null!;
    private Button openMapButton = null!;
    private Button callDispatcherButton = null!;

    private void InitializeComponent()
    {
        infoPanel = new Panel { Dock = DockStyle.Top, Height = 86, BackColor = UITheme.SurfaceMuted, Padding = new Padding(14) };
        var title = UITheme.CreateLabel("WR-10241  ·  Запланирован  ·  28.11.2026 12:00–15:00", 11F, FontStyle.Bold);
        title.Dock = DockStyle.Top;
        var details = UITheme.CreateLabel("Курьер Алексей  ·  Ford Transit А001АА78  ·  Центральный склад", 9F, FontStyle.Regular, UITheme.TextMuted);
        details.Dock = DockStyle.Top;
        infoPanel.Controls.Add(title);
        infoPanel.Controls.Add(details);

        var actions = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, FlowDirection = FlowDirection.LeftToRight };
        openMapButton = UITheme.CreateButton("Открыть карту", false, 140);
        openMapButton.Click += openMapButton_Click;
        actions.Controls.Add(openMapButton);
        callDispatcherButton = UITheme.CreateButton("Позвонить диспетчеру", true, 180);
        callDispatcherButton.Click += callDispatcherButton_Click;
        actions.Controls.Add(callDispatcherButton);

        itemsGrid = new DataGridView();
        UITheme.StyleGrid(itemsGrid);
        itemsGrid.Columns.Add("Product", "Товар");
        itemsGrid.Columns.Add("Ordered", "Заказано");
        itemsGrid.Columns.Add("Status", "Статус");
        itemsGrid.Rows.Add("Вода питьевая 19 л", "4", "Ожидает доставки");
        itemsGrid.Rows.Add("Вода питьевая 5 л", "6", "Ожидает доставки");
        itemsGrid.Rows.Add("Помпа механическая", "1", "Ожидает доставки");

        mapControl = new RouteMapControl();

        Content.Controls.Add(mapControl);
        Content.Controls.Add(itemsGrid);
        Content.Controls.Add(actions);
        Content.Controls.Add(infoPanel);
        Content.Controls.SetChildIndex(infoPanel, 0);
        Content.Controls.SetChildIndex(actions, 1);
        Content.Controls.SetChildIndex(itemsGrid, 2);
        Content.Controls.SetChildIndex(mapControl, 3);
    }
}