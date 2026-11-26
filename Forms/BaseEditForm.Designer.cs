using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class BaseEditForm
{
    protected Panel headerPanel = null!;
    protected Label titleLabel = null!;
    protected Label subtitleLabel = null!;
    protected Panel contentPanel = null!;
    protected FlowLayoutPanel footerPanel = null!;
    protected Button saveButton = null!;
    protected Button cancelButton = null!;

    private void InitializeComponent()
    {
        headerPanel = new Panel();
        titleLabel = UITheme.CreateLabel("Форма", 18F, FontStyle.Bold);
        subtitleLabel = UITheme.CreateLabel("Описание формы", 9F, FontStyle.Regular, UITheme.TextMuted);
        contentPanel = new Panel();
        footerPanel = new FlowLayoutPanel();
        saveButton = UITheme.CreateButton("Сохранить", true, 125);
        cancelButton = UITheme.CreateButton("Отмена", false, 105);
        headerPanel.SuspendLayout();
        footerPanel.SuspendLayout();
        SuspendLayout();
        // headerPanel
        headerPanel.BackColor = UITheme.Surface;
        headerPanel.Controls.Add(titleLabel);
        headerPanel.Controls.Add(subtitleLabel);
        headerPanel.Dock = DockStyle.Top;
        headerPanel.Height = 84;
        headerPanel.Padding = new Padding(22, 12, 22, 8);
        // titleLabel
        titleLabel.Location = new Point(22, 12);
        // subtitleLabel
        subtitleLabel.Location = new Point(24, 50);
        // contentPanel
        contentPanel.AutoScroll = true;
        contentPanel.BackColor = UITheme.Canvas;
        contentPanel.Dock = DockStyle.Fill;
        contentPanel.Padding = new Padding(22);
        // footerPanel
        footerPanel.BackColor = UITheme.Surface;
        footerPanel.Controls.Add(saveButton);
        footerPanel.Controls.Add(cancelButton);
        footerPanel.Dock = DockStyle.Bottom;
        footerPanel.FlowDirection = FlowDirection.RightToLeft;
        footerPanel.Height = 58;
        footerPanel.Padding = new Padding(10, 8, 14, 4);
        // saveButton
        saveButton.Click += SaveButton_Click;
        // cancelButton
        cancelButton.DialogResult = DialogResult.Cancel;
        // BaseEditForm
        AcceptButton = saveButton;
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UITheme.Canvas;
        CancelButton = cancelButton;
        ClientSize = new Size(780, 650);
        Controls.Add(contentPanel);
        Controls.Add(footerPanel);
        Controls.Add(headerPanel);
        Font = UITheme.BodyFont();
        KeyPreview = true;
        MinimumSize = new Size(680, 560);
        Name = "BaseEditForm";
        StartPosition = FormStartPosition.CenterParent;
        headerPanel.ResumeLayout(false);
        headerPanel.PerformLayout();
        footerPanel.ResumeLayout(false);
        ResumeLayout(false);
    }
}
