using WaterRoute.Forms;
using WaterRoute.Models;

namespace WaterRoute.Controls;

public partial class MonitorPage : BasePage
{
    private readonly System.Windows.Forms.Timer _refreshTimer;
    private bool _refreshing;

    public MonitorPage() : base(
        "Монитор маршрутов",
        "Состояние заказов и маршрутов. Автоматическое обновление каждые 30 секунд")
    {
        InitializeComponent();
        _refreshTimer = new System.Windows.Forms.Timer { Interval = 30000 };
        _refreshTimer.Tick += async (_, _) => await RefreshDataAsync();
        Load += (_, _) => _refreshTimer.Start();
        Disposed += (_, _) => _refreshTimer.Dispose();
    }

    private async void refreshButton_Click(object? sender, EventArgs e) => await RefreshDataAsync();

    private async Task RefreshDataAsync()
    {
        if (_refreshing || IsDisposed) return;
        _refreshing = true;
        refreshButton.Enabled = false;
        try
        {
            // Точка подключения sp_monitor_get. Локальная фильтрация выполняется после получения данных.
            await Task.Delay(50);
            if (IsDisposed) return;
            updatedLabel.Text = $"Обновлено: {DateTime.Now:HH:mm:ss}";
        }
        finally
        {
            _refreshing = false;
            if (!IsDisposed)
                refreshButton.Enabled = true;
        }
    }

    private void openRouteButton_Click(object? sender, EventArgs e)
    {
        using var form = new RoutePreviewForm();
        form.ShowDialog(this);
    }

    private void problemButton_Click(object? sender, EventArgs e)
    {
        using var form = new ProblemEditForm(UserRole.Dispatcher);
        form.ShowDialog(this);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.F5)
        {
            _ = RefreshDataAsync();
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }
}
