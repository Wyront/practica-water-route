using WaterRoute.Controls;
using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class RoutePreviewForm
{
    private Panel infoPanel = null!;
    private Label titleInfoLabel = null!;
    private Label detailsLabel = null!;
    private RouteMapControl routeMapControl = null!;
    private FlowLayoutPanel actionsPanel = null!;
    private Button pdfButton = null!;
    private Button offlineButton = null!;

    private void InitializeComponent()
    {
        infoPanel = new Panel(); titleInfoLabel = new Label(); detailsLabel = new Label(); routeMapControl = new RouteMapControl(); actionsPanel = new FlowLayoutPanel(); pdfButton = new Button(); offlineButton = new Button();
        infoPanel.SuspendLayout(); actionsPanel.SuspendLayout(); SuspendLayout();
        SetFormHeader("Маршрут R-021", "Просмотр направления доставки, статуса и временного окна"); ClientSize = new Size(1100, 760);
        infoPanel.Dock = DockStyle.Top; infoPanel.Height = 74; infoPanel.BackColor = UITheme.SurfaceMuted; infoPanel.Padding = new Padding(12);
        titleInfoLabel.Dock = DockStyle.Top; titleInfoLabel.Text = "WR-10241 · Клиент №1 · Запланирован"; titleInfoLabel.Font = UITheme.BodyFont(11F, FontStyle.Bold);
        detailsLabel.Dock = DockStyle.Top; detailsLabel.Text = "Курьер Алексей · Ford Transit А001АА78 · 12:00–15:00 · Центральный склад"; detailsLabel.ForeColor = UITheme.TextMuted;
        infoPanel.Controls.Add(detailsLabel); infoPanel.Controls.Add(titleInfoLabel);
        actionsPanel.Dock = DockStyle.Bottom; actionsPanel.Height = 48;
        pdfButton.Width = 205; pdfButton.Height = 38; pdfButton.Text = "Экспорт маршрута в PDF"; pdfButton.FlatStyle = FlatStyle.Flat;
        offlineButton.Width = 205; offlineButton.Height = 38; offlineButton.Text = "Проверить офлайн-режим"; offlineButton.FlatStyle = FlatStyle.Flat; offlineButton.Click += offlineButton_Click;
        actionsPanel.Controls.Add(pdfButton); actionsPanel.Controls.Add(offlineButton);
        contentPanel.Controls.Add(routeMapControl); contentPanel.Controls.Add(actionsPanel); contentPanel.Controls.Add(infoPanel);
        infoPanel.ResumeLayout(false); actionsPanel.ResumeLayout(false); ResumeLayout(false);
    }
}
