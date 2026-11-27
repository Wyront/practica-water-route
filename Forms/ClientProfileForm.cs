using Microsoft.Extensions.DependencyInjection;
using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Forms;

public sealed partial class ClientProfileForm : BaseEditForm
{
    private readonly IRepository _repo;
    private readonly IServiceProvider _services;
    private readonly INotificationService _notifications;
    private int _clientId = -1;

    public ClientProfileForm(
        IRepository repo,
        IServiceProvider services,
        INotificationService notifications)
        : base("Профиль и адреса", "Личные данные, адресная книга и долг по таре", 900, 700)
    {
        _repo = repo;
        _services = services;
        _notifications = notifications;
        InitializeComponent();
        Load += (_, _) => LoadProfile();
    }

    // Для визуального дизайнера
    public ClientProfileForm() : this(null!, null!, null!) { }

    public void LoadProfile(int clientId)
    {
        _clientId = clientId;
        LoadProfile();
    }

    private async void LoadProfile()
    {
        if (_repo == null || _clientId <= 0) return;

        try
        {
            var client = await _repo.QuerySingleAsync<dynamic>("sp_client_get", new { p_client_id = _clientId });
            if (client != null)
            {
                nameTextBox.Text = client["full_name"]?.ToString() ?? "";
                emailTextBox.Text = client["email"]?.ToString() ?? "";
                phoneTextBox.Text = client["phone"]?.ToString() ?? "";
            }

            await LoadAddresses();
            await LoadContainerDebt();
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка загрузки профиля", ex.Message);
        }
    }

    private async Task LoadAddresses()
    {
        if (_repo == null || _clientId <= 0) return;

        try
        {
            var rows = await _repo.QueryAsync<dynamic>("sp_client_address_list", new { p_client_id = _clientId });
            
            addressesGrid.Rows.Clear();
            foreach (IDictionary<string, object?> r in rows)
            {
                var fullAddress = r["full_address"]?.ToString() 
                    ?? $"{r["city"]} {r["street"]} {r["building"]}" + (string.IsNullOrEmpty(r["apartment"]?.ToString()) ? "" : $", кв. {r["apartment"]}");
                var coords = (r["latitude"] != null && r["longitude"] != null)
                    ? $"{Convert.ToDecimal(r["latitude"]):0.0000}, {Convert.ToDecimal(r["longitude"]):0.0000}"
                    : "—";

                var index = addressesGrid.Rows.Add(
                    r["id"],
                    fullAddress,
                    r["warehouse_name"]?.ToString() ?? "—",
                    coords,
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

    private async Task LoadContainerDebt()
    {
        if (_repo == null || _clientId <= 0) return;

        try
        {
            var rows = await _repo.QueryAsync<dynamic>("sp_container_debt_get", new { p_client_id = _clientId });
            var parts = new List<string>();
            foreach (IDictionary<string, object?> r in rows)
            {
                var name = r["water_type_name"]?.ToString() ?? "";
                var issued = Convert.ToInt32(r["issued_qty"]);
                var returned = Convert.ToInt32(r["confirmed_returned_qty"]);
                var pending = Convert.ToInt32(r["pending_returned_qty"]);
                var debt = Convert.ToInt32(r["current_debt"]);
                if (debt > 0)
                    parts.Add($"{name}: {debt} шт (на руках: {issued}, возвращено: {returned}, ожидает: {pending})");
            }
            debtLabel.Text = parts.Count > 0 
                ? "Долг по таре: " + string.Join(" | ", parts)
                : "Долг по таре: нет задолженности";
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка загрузки долга по таре", ex.Message);
        }
    }

    private void editProfileButton_Click(object? sender, EventArgs e)
    {
        nameTextBox.ReadOnly = false;
        phoneTextBox.ReadOnly = false;
        saveProfileButton.Visible = true;
        editProfileButton.Visible = false;
    }

    private async void saveProfileButton_Click(object? sender, EventArgs e)
    {
        if (_repo == null || _clientId <= 0) return;

        if (string.IsNullOrWhiteSpace(nameTextBox.Text) || string.IsNullOrWhiteSpace(emailTextBox.Text))
        {
            _notifications.ShowError("Ошибка", "Заполните ФИО и e-mail");
            return;
        }

        try
        {
            await _repo.ExecuteAsync("sp_client_update", new
            {
                p_client_id = _clientId,
                p_full_name = nameTextBox.Text.Trim(),
                p_phone = phoneTextBox.Text.Trim(),
                p_active = 1
            });
            _notifications.ShowSuccess("Профиль", "Профиль успешно сохранён");
            nameTextBox.ReadOnly = true;
            phoneTextBox.ReadOnly = true;
            saveProfileButton.Visible = false;
            editProfileButton.Visible = true;
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка сохранения", ex.Message);
        }
    }

    private void addAddressButton_Click(object? sender, EventArgs e)
    {
        using var form = _services.GetRequiredService<AddressEditForm>();
        form.SetClientId(_clientId);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            LoadProfile();
        }
    }

    private void editAddressButton_Click(object? sender, EventArgs e)
    {
        if (addressesGrid.SelectedRows.Count == 0) return;
        var row = (IDictionary<string, object?>)addressesGrid.SelectedRows[0].Tag!;
        var addressId = Convert.ToInt32(row["id"]);
        
        using var form = new AddressEditForm(_repo, _services, _notifications, addressId, _clientId);
        if (form.ShowDialog(this) == DialogResult.OK)
            LoadProfile();
    }

    private async void setDefaultButton_Click(object? sender, EventArgs e)
    {
        if (addressesGrid.SelectedRows.Count == 0) return;
        var row = (IDictionary<string, object?>)addressesGrid.SelectedRows[0].Tag!;
        var addressId = Convert.ToInt32(row["id"]);
        
        try
        {
            await _repo.ExecuteAsync("sp_client_address_set_default", new { p_address_id = addressId, p_client_id = _clientId });
            _notifications.ShowSuccess("Адрес", "Адрес по умолчанию установлен");
            LoadProfile();
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка", ex.Message);
        }
    }

    private async void deactivateAddressButton_Click(object? sender, EventArgs e)
    {
        if (addressesGrid.SelectedRows.Count == 0) return;
        if (MessageBox.Show("Деактивировать адрес? Он останется в истории заказов.", "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        var row = (IDictionary<string, object?>)addressesGrid.SelectedRows[0].Tag!;
        var addressId = Convert.ToInt32(row["id"]);
        
        try
        {
            // Load current address data first, then update with active=0
            var currentAddr = await _repo.QueryAsync<dynamic>("sp_client_address_list", new { p_client_id = _clientId });
            var addrList = currentAddr?.ToList() ?? new List<dynamic>();
            var addr = addrList.FirstOrDefault(r => Convert.ToInt32(r["id"]) == addressId);
            if (addr != null)
            {
                await _repo.ExecuteAsync("sp_client_address_update", new 
                { 
                    p_address_id = addressId, 
                    p_client_id = _clientId,
                    p_warehouse_id = Convert.ToInt32(addr["warehouse_id"]),
                    p_city = addr["city"]?.ToString() ?? "",
                    p_street = addr["street"]?.ToString() ?? "",
                    p_building = addr["building"]?.ToString() ?? "",
                    p_apartment = addr["apartment"]?.ToString() ?? "",
                    p_latitude = addr["latitude"] != null ? Convert.ToDecimal(addr["latitude"]) : (decimal?)null,
                    p_longitude = addr["longitude"] != null ? Convert.ToDecimal(addr["longitude"]) : (decimal?)null,
                    p_is_default = 0, 
                    p_active = 0
                });
                _notifications.ShowSuccess("Адрес", "Адрес деактивирован");
                LoadProfile();
            }
            else
            {
                _notifications.ShowError("Ошибка", "Адрес не найден");
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка", ex.Message);
        }
    }
}