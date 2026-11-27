using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WaterRoute.Infrastructure;

namespace WaterRoute.Infrastructure;

public sealed class ToastNotificationService : INotificationService
{
    private Form? _owner;
    private readonly List<ToastForm> _activeToasts = [];
    private const int MaxToasts = 5;
    private const int ToastWidth = 360;
    private const int ToastHeight = 100;
    private const int Margin = 12;

    public ToastNotificationService()
    {
    }

    public ToastNotificationService(Form owner)
    {
        _owner = owner;
    }

    public void SetOwner(Form owner) => _owner = owner;

    public void ReleaseOwner(Form owner)
    {
        if (ReferenceEquals(_owner, owner))
            _owner = null;
    }

    public void ShowToast(string title, string message, ToastType type = ToastType.Info, int durationMs = 5000)
    {
        var owner = _owner;
        if (owner == null || owner.IsDisposed)
            return;

        if (owner.InvokeRequired)
        {
            owner.Invoke(() => ShowToast(title, message, type, durationMs));
            return;
        }

        CleanupClosedToasts();

        while (_activeToasts.Count >= MaxToasts)
        {
            var oldest = _activeToasts[0];
            oldest.Close();
            _activeToasts.RemoveAt(0);
        }

        var toast = new ToastForm(title, message, type, durationMs);
        PositionToast(toast);
        toast.FormClosed += (_, _) => _activeToasts.Remove(toast);
        _activeToasts.Add(toast);
        toast.Show(owner);
    }

    public void ShowError(string title, string message, int durationMs = 8000)
        => ShowToast(title, message, ToastType.Error, durationMs);

    public void ShowWarning(string title, string message, int durationMs = 6000)
        => ShowToast(title, message, ToastType.Warning, durationMs);

    public void ShowSuccess(string title, string message, int durationMs = 4000)
        => ShowToast(title, message, ToastType.Success, durationMs);

    private void CleanupClosedToasts()
    {
        _activeToasts.RemoveAll(t => t.IsDisposed || !t.Visible);
    }

    private void PositionToast(ToastForm toast)
    {
        var screen = Screen.FromControl(_owner!);
        var workArea = screen.WorkingArea;
        var x = workArea.Right - ToastWidth - Margin;
        var y = workArea.Bottom - Margin;

        foreach (var existing in _activeToasts)
        {
            y -= existing.Height + Margin;
        }

        toast.Location = new Point(x, y);
    }
}

internal sealed class ToastForm : Form
{
    private readonly System.Windows.Forms.Timer _timer;
    private readonly ToastType _type;

    public ToastForm(string title, string message, ToastType type, int durationMs)
    {
        _type = type;
        FormBorderStyle = FormBorderStyle.None;
        Size = new Size(360, 100);
        StartPosition = FormStartPosition.Manual;
        ShowInTaskbar = false;
        TopMost = true;
        BackColor = UITheme.Surface;
        DoubleBuffered = true;

        var borderColor = type switch
        {
            ToastType.Success => UITheme.Success,
            ToastType.Warning => UITheme.Warning,
            ToastType.Error => UITheme.Danger,
            _ => UITheme.Primary
        };

        var borderPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = 4,
            BackColor = borderColor
        };

        var content = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
        var titleLabel = UITheme.CreateLabel(title, 10F, FontStyle.Bold);
        titleLabel.Dock = DockStyle.Top;
        var messageLabel = UITheme.CreateLabel(message, 9F, FontStyle.Regular, UITheme.TextMuted, false);
        messageLabel.Dock = DockStyle.Fill;
        content.Controls.Add(messageLabel);
        content.Controls.Add(titleLabel);

        var closeButton = new Button
        {
            Text = "✕",
            Size = new Size(24, 24),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent,
            ForeColor = UITheme.TextMuted,
            Font = UITheme.BodyFont(9F),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Cursor = Cursors.Hand
        };
        closeButton.FlatAppearance.BorderSize = 0;
        closeButton.Click += (_, _) => Close();
        content.Controls.Add(closeButton);

        Controls.Add(content);
        Controls.Add(borderPanel);

        _timer = new System.Windows.Forms.Timer { Interval = durationMs };
        _timer.Tick += (_, _) => { _timer.Stop(); Close(); };
        _timer.Start();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using var pen = new Pen(UITheme.Border);
        e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _timer.Dispose();
        base.Dispose(disposing);
    }
}