using Microsoft.Extensions.DependencyInjection;
using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Forms;

public partial class ClientEditForm : BaseEditForm
{
    private readonly IRepository _repo;
    private readonly IServiceProvider _services;
    private readonly INotificationService _notifications;
    private int _clientId = -1;

    public ClientEditForm(
        IRepository repo,
        IServiceProvider services,
        INotificationService notifications)
        : base("Карточка клиента", "Контактные данные и адресная книга клиента", 900, 700)
    {
        _repo = repo;
        _services = services;
        _notifications = notifications;
        InitializeComponent();
        Load += (_, _) => LoadAddresses();
    }

    // Для визуального дизайнера
    public ClientEditForm() : this(null!, null!, null!) { }

    public void LoadClient(int clientId)
    {
        _clientId = clientId;
        LoadClientData();
    }

    private async void LoadClientData()
    {
        if (_repo == null || _clientId <= 0) return;

        try
        {
            var client = await _repo.QuerySingleAsync<dynamic>("sp_client_get", new { p_client_id = _clientId });
            if (client != null)
            {
                fullNameTextBox.Text = client["full_name"]?.ToString() ?? "";
                emailTextBox.Text = client["email"]?.ToString() ?? "";
                phoneTextBox.Text = client["phone"]?.ToString() ?? "";
                activeCheckBox.Checked = Convert.ToBoolean(client["active"]);
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка загрузки клиента", ex.Message);
        }
    }

    private async void LoadAddresses()
    {
        if (_repo == null || _clientId <= 0) return;

        try
        {
            var rows = await _repo.QueryAsync<dynamic>("sp_client_address_list", new { p_client_id = _clientId });
            
            addressesGrid.Rows.Clear();
            foreach (IDictionary<string, object?> r in rows)
            {
                var index = addressesGrid.Rows.Add(
                    r["full_address"] ?? r["address"] ?? "—",
                    r["warehouse_name"] ?? "—",
                    Convert.ToBoolean(r["is_default"]) ? "Да" : "Нет",
                    Convert.ToBoolean(r["active"]) ? "Да" : "Нет"
                );
                addressesGrid.Rows[index].Tag = r;
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка загрузки адресов", ex.Message);
        }
    }

    private void addressButton_Click(object? sender, EventArgs e)
    {
        using var form = _services.GetRequiredService<AddressEditForm>();
        if (form.ShowDialog(this) == DialogResult.OK)
            LoadAddresses();
    }

    private async Task<bool> SaveClient()
    {
        if (_repo == null || _clientId <= 0) return false;

        if (string.IsNullOrWhiteSpace(fullNameTextBox.Text))
        {
            _notifications.ShowError("Ошибка", "Введите ФИО");
            return false;
        }

        try
        {
            await _repo.ExecuteAsync("sp_client_update", new
            {
                p_client_id = _clientId,
                p_full_name = fullNameTextBox.Text.Trim(),
                p_phone = phoneTextBox.Text.Trim(),
                p_active = activeCheckBox.Checked ? 1 : 0
            });
            _notifications.ShowSuccess("Клиент", "Данные клиента сохранены");
            return true;
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка сохранения", ex.Message);
            return false;
        }
    }

    protected override async Task<bool> ValidateAndSaveAsync() => await SaveClient();
}