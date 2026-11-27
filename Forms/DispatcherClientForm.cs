using Microsoft.Extensions.DependencyInjection;
using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Forms;

public sealed partial class DispatcherClientForm : BaseEditForm
{
    private readonly IRepository _repo;
    private readonly IServiceProvider _services;
    private readonly INotificationService _notifications;
    private readonly AuthService _auth;

    public DispatcherClientForm(
        IRepository repo,
        IServiceProvider services,
        AuthService auth,
        INotificationService notifications)
        : base("Работа с клиентами", "Создание клиентов, управление адресами и просмотр истории заказов", 1100, 700)
    {
        _repo = repo;
        _services = services;
        _auth = auth;
        _notifications = notifications;
        InitializeComponent();
        Load += (_, _) => RefreshData();
    }

    // Для визуального дизайнера
    public DispatcherClientForm() : this(null!, null!, null!, null!) { }

    private async void RefreshData()
    {
        if (_repo == null) return;
        
        try
        {
            var rows = await _repo.QueryAsync<dynamic>("sp_client_list", new { p_search = (string?)null, p_active = (int?)null, p_limit = 1000, p_offset = 0 });
            
            clientsGrid.Rows.Clear();
            foreach (IDictionary<string, object?> r in rows)
            {
                var index = clientsGrid.Rows.Add(
                    r["id"],
                    r["full_name"],
                    r["phone"],
                    r["addresses_count"],
                    r["orders_count"],
                    Convert.ToBoolean(r["active"]) ? "Да" : "Нет"
                );
                clientsGrid.Rows[index].Tag = r;
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка загрузки клиентов", ex.Message);
        }
    }

    private void createButton_Click(object? sender, EventArgs e)
    {
        if (_auth == null) return;
        using var form = new RegistrationForm(_auth);
        if (form.ShowDialog(this) == DialogResult.OK)
            RefreshData();
    }

    private void editButton_Click(object? sender, EventArgs e)
    {
        if (clientsGrid.SelectedRows.Count == 0) return;
        var row = (IDictionary<string, object?>)clientsGrid.SelectedRows[0].Tag!;
        var clientId = Convert.ToInt32(row["id"]);
        
        using var form = _services.GetRequiredService<ClientEditForm>();
        form.LoadClient(clientId);
        if (form.ShowDialog(this) == DialogResult.OK)
            RefreshData();
    }

    private void addressesButton_Click(object? sender, EventArgs e)
    {
        if (clientsGrid.SelectedRows.Count == 0) return;
        var row = (IDictionary<string, object?>)clientsGrid.SelectedRows[0].Tag!;
        var clientId = Convert.ToInt32(row["id"]);
        
        using var form = _services.GetRequiredService<ClientProfileForm>();
        // Create a session-like object for the client
        var clientSession = new UserSession(clientId, row["email"]?.ToString() ?? "", row["full_name"]?.ToString() ?? "", UserRole.Client);
        form.LoadProfile(clientId);
        form.ShowDialog(this);
        RefreshData();
    }

    private void historyButton_Click(object? sender, EventArgs e)
    {
        if (clientsGrid.SelectedRows.Count == 0) return;
        var row = (IDictionary<string, object?>)clientsGrid.SelectedRows[0].Tag!;
        var clientId = Convert.ToInt32(row["id"]);
        
        using var form = _services.GetRequiredService<ClientOrderHistoryForm>();
        form.LoadHistory(clientId);
        form.ShowDialog(this);
    }

    private void callButton_Click(object? sender, EventArgs e)
    {
        if (clientsGrid.SelectedRows.Count == 0) return;
        var row = (IDictionary<string, object?>)clientsGrid.SelectedRows[0].Tag!;
        var phone = row["phone"]?.ToString() ?? "";
        if (MessageBox.Show($"Позвонить клиенту по номеру {phone}?", "Звонок",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            Clipboard.SetText(phone);
            _notifications.ShowSuccess("Звонок", $"Номер {phone} скопирован в буфер обмена");
        }
    }
}