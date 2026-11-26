#if NET10_0_OR_GREATER
#nullable enable
#endif

using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class OfflineCallForm
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    private Label iconLabel = null!;
    private Label titleLabel = null!;
    private Label messageLabel = null!;
    private Label dispatcherLabel = null!;
    private Label phoneLabel = null!;
    private Button copyButton = null!;
    private Button closeButton = null!;

    private void InitializeComponent()
    {
        iconLabel = new Label(); titleLabel = new Label(); messageLabel = new Label(); dispatcherLabel = new Label(); phoneLabel = new Label();
        copyButton = new Button(); closeButton = new Button(); SuspendLayout();
        iconLabel.Font = UITheme.TitleFont(32F); iconLabel.ForeColor = UITheme.Warning; iconLabel.SetBounds(224, 28, 90, 70); iconLabel.Text = "!"; iconLabel.TextAlign = ContentAlignment.MiddleCenter;
        titleLabel.AutoSize = true; titleLabel.Font = UITheme.TitleFont(17F); titleLabel.Location = new Point(92, 110); titleLabel.Text = "Нет соединения с сервером";
        messageLabel.Font = UITheme.BodyFont(10F); messageLabel.ForeColor = UITheme.TextMuted; messageLabel.SetBounds(55, 155, 430, 55);
        messageLabel.Text = "Изменение данных временно недоступно. Позвоните дежурному диспетчеру по резервному номеру."; messageLabel.TextAlign = ContentAlignment.MiddleCenter;
        dispatcherLabel.AutoSize = true; dispatcherLabel.Font = UITheme.BodyFont(11F, FontStyle.Bold); dispatcherLabel.Location = new Point(170, 218); dispatcherLabel.Text = "Диспетчер Анна";
        phoneLabel.AutoSize = true; phoneLabel.Font = UITheme.TitleFont(16F); phoneLabel.ForeColor = UITheme.Primary; phoneLabel.Location = new Point(140, 250); phoneLabel.Text = "+7 900 000-00-02";
        copyButton.SetBounds(82, 302, 180, 38); copyButton.Text = "Копировать номер"; copyButton.BackColor = UITheme.Primary; copyButton.ForeColor = Color.White; copyButton.FlatStyle = FlatStyle.Flat; copyButton.Click += copyButton_Click;
        closeButton.SetBounds(300, 302, 120, 38); closeButton.Text = "Закрыть"; closeButton.DialogResult = DialogResult.Cancel; closeButton.FlatStyle = FlatStyle.Flat;
        AutoScaleMode = AutoScaleMode.Font; BackColor = UITheme.Surface; CancelButton = closeButton; ClientSize = new Size(540, 380);
        Controls.Add(iconLabel); Controls.Add(titleLabel); Controls.Add(messageLabel); Controls.Add(dispatcherLabel); Controls.Add(phoneLabel); Controls.Add(copyButton); Controls.Add(closeButton);
        Font = UITheme.BodyFont(); MaximizeBox = false; MinimumSize = new Size(520, 360); Name = "OfflineCallForm"; StartPosition = FormStartPosition.CenterParent; Text = "Нет соединения — WaterRoute";
        ResumeLayout(false); PerformLayout();
    }
}
