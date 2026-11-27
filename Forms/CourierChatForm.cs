using Microsoft.Extensions.DependencyInjection;
using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Forms;

public sealed partial class CourierChatForm : BaseEditForm
{
    private readonly IRepository _repo;
    private readonly IServiceProvider _services;
    private readonly INotificationService _notifications;
    private int _routeId = -1;
    private int _courierId = -1;
    private int _dispatcherId = -1;
    private CancellationTokenSource? _refreshCts;
    private readonly List<IDictionary<string, object?>> _routeData = new();

    public CourierChatForm(
        IRepository repo,
        IServiceProvider services,
        INotificationService notifications)
        : base("Чат с диспетчером", "Сообщения курьера и назначенного диспетчера", 800, 600)
    {
        _repo = repo;
        _services = services;
        _notifications = notifications;
        InitializeComponent();
        Load += (_, _) => LoadRoutes();
        FormClosed += (_, _) => _refreshCts?.Cancel();
    }

    // Для визуального дизайнера
    public CourierChatForm() : this(null!, null!, null!) { }

    public void LoadChat(int courierId, int routeId = -1)
    {
        _courierId = courierId;
        _routeId = routeId;
        LoadRoutes();
        if (_routeId > 0) LoadChat();
    }

    private async void LoadRoutes()
    {
        if (_repo == null) return;

        try
        {
            var rows = await _repo.QueryAsync<dynamic>("sp_route_list", new
            {
                p_courier_id = _courierId,
                p_status = (string?)null,
                p_dispatcher_id = (int?)null,
                p_date_from = (DateTime?)null,
                p_date_to = (DateTime?)null,
                p_limit = 100,
                p_offset = 0
            });

            routesListBox.Items.Clear();
            _routeData.Clear();
            foreach (IDictionary<string, object?> r in rows)
            {
                var status = r["status"]?.ToString() ?? "";
                var display = $"{r["route_number"]} · {r["order_number"]} · {GetStatusDisplay(status)}";
                routesListBox.Items.Add(display);
                _routeData.Add(r);
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка загрузки маршрутов", ex.Message);
        }
    }

    private static string GetStatusDisplay(string? status) => status switch
    {
        "planned" => "запланирован",
        "in_progress" => "активный",
        "completed" => "завершён",
        "cancelled" => "отменён",
        _ => status ?? ""
    };

    private void routesListBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (routesListBox.SelectedIndex < 0) return;
        var row = _routeData[routesListBox.SelectedIndex];
        _routeId = Convert.ToInt32(row["id"]);
        _dispatcherId = Convert.ToInt32(row["dispatcher_id"]);
        routeTitleLabel.Text = $"{row["route_number"]} · {row["order_number"]}";
        participantsLabel.Text = $"{row["courier_name"]} ↔ {row["dispatcher_name"]} · история сохраняется";
        LoadChat();
    }

    private async void LoadChat()
    {
        if (_repo == null || _routeId <= 0) return;

        try
        {
            var rows = await _repo.QueryAsync<dynamic>("sp_chat_get", new
            {
                p_user_id = _courierId,
                p_other_user_id = _dispatcherId,
                p_limit = 100,
                p_offset = 0
            });

            messagesRichTextBox.Clear();
            foreach (IDictionary<string, object?> r in rows.OrderBy(x => Convert.ToDateTime(x["created_at"])))
            {
                var senderName = Convert.ToInt32(r["sender_id"]) == _courierId ? "Курьер" : "Диспетчер";
                var time = Convert.ToDateTime(r["created_at"]).ToString("HH:mm");
                messagesRichTextBox.AppendText($"[{time}] {senderName}:\n{r["message"]}\n\n");
            }
            messagesRichTextBox.SelectionStart = messagesRichTextBox.TextLength;
            messagesRichTextBox.ScrollToCaret();
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка загрузки чата", ex.Message);
        }
    }

    private async void sendButton_Click(object? sender, EventArgs e)
    {
        if (_repo == null || _routeId <= 0 || _dispatcherId <= 0) return;
        if (string.IsNullOrWhiteSpace(inputTextBox.Text)) return;

        var message = inputTextBox.Text.Trim();
        inputTextBox.Clear();

        try
        {
            await _repo.ExecuteAsync("sp_chat_send", new
            {
                p_sender_id = _courierId,
                p_recipient_id = _dispatcherId,
                p_message = message
            });
            await _repo.ExecuteAsync("sp_chat_mark_read", new { p_user_id = _courierId, p_other_user_id = _dispatcherId });
            LoadChat();
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка отправки", ex.Message);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshCts?.Cancel();
            _refreshCts?.Dispose();
        }
        base.Dispose(disposing);
    }
}