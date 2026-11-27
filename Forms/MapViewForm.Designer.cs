using Mapsui.UI.WindowsForms;
using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class MapViewForm
{
    private Button closeButton = null!;

    private void InitializeComponent()
    {
        _mapControl = new MapControl { Dock = DockStyle.Fill };
        closeButton = UITheme.CreateButton("Закрыть", false, 120);
        closeButton.DialogResult = DialogResult.Cancel;

        var toolbar = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = UITheme.Surface, Padding = new Padding(12) };
        var routeLabel = UITheme.CreateLabel("R-021 · WR-10241 · В работе", 12F, FontStyle.Bold);
        routeLabel.Location = new Point(12, 8);
        toolbar.Controls.Add(routeLabel);
        var detailsLabel = UITheme.CreateLabel("Курьер Алексей · Ford Transit А001АА78 · 12:00–15:00", 9F, FontStyle.Regular, UITheme.TextMuted);
        detailsLabel.Location = new Point(12, 36);
        toolbar.Controls.Add(detailsLabel);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 52, FlowDirection = FlowDirection.RightToLeft };
        closeButton.Location = new Point(0, 4);
        buttons.Controls.Add(closeButton);

        var mapStatus = UITheme.CreateLabel("© OpenStreetMap contributors · онлайн-карта", 9F, FontStyle.Regular, UITheme.TextMuted);
        mapStatus.Dock = DockStyle.Bottom;
        mapStatus.Height = 30;
        mapStatus.BackColor = UITheme.Surface;
        mapStatus.Padding = new Padding(12, 6, 12, 4);

        Content.Controls.Add(_mapControl);
        Content.Controls.Add(mapStatus);
        Content.Controls.Add(buttons);
        Content.Controls.Add(toolbar);
        Content.Controls.SetChildIndex(toolbar, 0);
        Content.Controls.SetChildIndex(buttons, 1);
        Content.Controls.SetChildIndex(mapStatus, 2);
        Content.Controls.SetChildIndex(_mapControl, 3);
    }
}