using WaterRoute.Infrastructure;

namespace WaterRoute.Controls;

partial class RouteMapPage
{
    private Panel toolbarPanel = null!;
    private Label routeLabel = null!;
    private ComboBox routeComboBox = null!;
    private Button offlineButton = null!;
    private Label statusLabel = null!;
    private RouteMapControl routeMapControl = null!;

    private void InitializeComponent()
    {
        toolbarPanel = new Panel();
        routeLabel = new Label();
        routeComboBox = new ComboBox();
        offlineButton = new Button();
        statusLabel = new Label();
        routeMapControl = new RouteMapControl();
        toolbarPanel.SuspendLayout();
        SuspendLayout();
        // toolbarPanel
        toolbarPanel.BackColor = UITheme.Surface;
        toolbarPanel.Controls.Add(routeLabel);
        toolbarPanel.Controls.Add(routeComboBox);
        toolbarPanel.Controls.Add(offlineButton);
        toolbarPanel.Dock = DockStyle.Top;
        toolbarPanel.Height = 78;
        toolbarPanel.Name = "toolbarPanel";
        // routeLabel
        routeLabel.AutoSize = true;
        routeLabel.Location = new Point(12, 5);
        routeLabel.Name = "routeLabel";
        routeLabel.Text = "Маршрут";
        // routeComboBox
        routeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        routeComboBox.Items.AddRange(new object[] { "R-021 · WR-10241", "R-018 · WR-10235" });
        routeComboBox.Name = "routeComboBox";
        routeComboBox.SelectedIndex = 0;
        routeComboBox.SetBounds(12, 28, 260, 34);
        // offlineButton
        offlineButton.FlatStyle = FlatStyle.Flat;
        offlineButton.Name = "offlineButton";
        offlineButton.SetBounds(602, 26, 180, 38);
        offlineButton.Text = "Потеря связи";
        offlineButton.Click += offlineButton_Click;
        // statusLabel
        statusLabel.BackColor = UITheme.Surface;
        statusLabel.Dock = DockStyle.Bottom;
        statusLabel.ForeColor = UITheme.TextMuted;
        statusLabel.Height = 38;
        statusLabel.Name = "statusLabel";
        statusLabel.Padding = new Padding(12, 8, 12, 4);
        statusLabel.Text = "© OpenStreetMap contributors · онлайн-карта";
        // routeMapControl
        routeMapControl.Dock = DockStyle.Fill;
        routeMapControl.Name = "routeMapControl";
        // RouteMapPage
        Content.Controls.Add(routeMapControl);
        Content.Controls.Add(statusLabel);
        Content.Controls.Add(toolbarPanel);
        Name = "RouteMapPage";
        Size = new Size(1000, 700);
        toolbarPanel.ResumeLayout(false);
        toolbarPanel.PerformLayout();
        ResumeLayout(false);
    }
}