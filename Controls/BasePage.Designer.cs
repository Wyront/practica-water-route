using WaterRoute.Infrastructure;

namespace WaterRoute.Controls;

partial class BasePage
{
    protected Panel headerPanel = null!;
    protected Label titleLabel = null!;
    protected Label subtitleLabel = null!;
    protected Panel contentPanel = null!;

    private void InitializeComponent()
    {
        headerPanel = new Panel();
        titleLabel = UITheme.CreateLabel("Раздел", 20F, FontStyle.Bold);
        subtitleLabel = UITheme.CreateLabel("Описание раздела", 9.5F, FontStyle.Regular, UITheme.TextMuted);
        contentPanel = new Panel();
        headerPanel.SuspendLayout();
        SuspendLayout();
        // headerPanel
        headerPanel.BackColor = UITheme.Surface;
        headerPanel.Controls.Add(titleLabel);
        headerPanel.Controls.Add(subtitleLabel);
        headerPanel.Dock = DockStyle.Top;
        headerPanel.Height = 94;
        headerPanel.Padding = new Padding(24, 15, 24, 10);
        // titleLabel
        titleLabel.Location = new Point(24, 14);
        // subtitleLabel
        subtitleLabel.Location = new Point(26, 55);
        subtitleLabel.MaximumSize = new Size(1000, 0);
        // contentPanel
        contentPanel.BackColor = UITheme.Canvas;
        contentPanel.Dock = DockStyle.Fill;
        contentPanel.Padding = new Padding(20);
        // BasePage
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UITheme.Canvas;
        Controls.Add(contentPanel);
        Controls.Add(headerPanel);
        Dock = DockStyle.Fill;
        Font = UITheme.BodyFont();
        Name = "BasePage";
        Size = new Size(1000, 700);
        headerPanel.ResumeLayout(false);
        headerPanel.PerformLayout();
        ResumeLayout(false);
    }
}
