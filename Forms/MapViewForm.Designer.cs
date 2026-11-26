using Mapsui.UI.WindowsForms;
using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class MapViewForm
{
    private Button offlineModeButton = null!;
    private Button cacheButton = null!;
    private Button closeButton = null!;

    private void InitializeComponent()
    {
        _mapControl = new MapControl { Dock = DockStyle.Fill };
        offlineModeButton = UITheme.CreateButton("Офлайн-режим (MBTiles)", false, 190);
        offlineModeButton.Click += offlineModeButton_Click;
        cacheButton = UITheme.CreateButton("Обновить кэш", false, 150);
        cacheButton.Click += cacheButton_Click;
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
        offlineModeButton.Location = new Point(0, 4);
        cacheButton.Location = new Point(200, 4);
        closeButton.Location = new Point(360, 4);
        buttons.Controls.Add(offlineModeButton);
        buttons.Controls.Add(cacheButton);
        buttons.Controls.Add(closeButton);

        var cacheStatus = UITheme.CreateLabel("Кэш: последний сохранённый маршрут доступен без сети", 9F, FontStyle.Regular, UITheme.TextMuted);
        cacheStatus.Dock = DockStyle.Bottom;
        cacheStatus.Height = 30;
        cacheStatus.BackColor = UITheme.Surface;
        cacheStatus.Padding = new Padding(12, 6, 12, 4);

        Content.Controls.Add(_mapControl);
        Content.Controls.Add(cacheStatus);
        Content.Controls.Add(buttons);
        Content.Controls.Add(toolbar);
        Content.Controls.SetChildIndex(toolbar, 0);
        Content.Controls.SetChildIndex(buttons, 1);
        Content.Controls.SetChildIndex(cacheStatus, 2);
        Content.Controls.SetChildIndex(_mapControl, 3);
    }
}