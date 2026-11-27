using Microsoft.Extensions.DependencyInjection;
using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Forms;

public partial class AddressEditForm : BaseEditForm
{
    private readonly IRepository _repo;
    private readonly IServiceProvider _services;
    private readonly INotificationService _notifications;
    private int _addressId = -1;
    private int _clientId = -1;
    private List<WarehouseInfo> _warehouses = new();

    public AddressEditForm(
        IRepository repo,
        IServiceProvider services,
        INotificationService notifications)
        : base("Новый адрес", "Адрес привязывается к складу и точке на карте Санкт-Петербурга", 760, 650)
    {
        _repo = repo;
        _services = services;
        _notifications = notifications;
        InitializeComponent();
        Load += (_, _) => LoadWarehouses();
    }

    // Для визуального дизайнера
    public AddressEditForm() : this(null!, null!, null!) { }

    public AddressEditForm(
        IRepository repo,
        IServiceProvider services,
        INotificationService notifications,
        int addressId, int clientId)
        : base("Редактирование адреса", "Адрес привязывается к складу и точке на карте Санкт-Петербурга", 760, 650)
    {
        _repo = repo;
        _services = services;
        _notifications = notifications;
        _addressId = addressId;
        _clientId = clientId;
        InitializeComponent();
        Load += (_, _) => { LoadWarehouses(); LoadAddress(); };
    }

    private async void LoadWarehouses()
    {
        if (_repo == null) return;

        try
        {
            var rows = await _repo.QueryAsync<dynamic>("sp_directories_get_active", new { p_type = "warehouse" });
            _warehouses.Clear();
            warehouseComboBox.Items.Clear();
            foreach (IDictionary<string, object?> r in rows)
            {
                var wh = new WarehouseInfo(
                    Convert.ToInt32(r["id"]),
                    r["name"]?.ToString() ?? ""
                );
                _warehouses.Add(wh);
                warehouseComboBox.Items.Add(wh.Name);
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка загрузки складов", ex.Message);
        }
    }

    public async void LoadAddress()
    {
        if (_repo == null || _addressId <= 0) return;

        try
        {
            var rows = await _repo.QueryAsync<dynamic>("sp_client_address_list", new { p_client_id = _clientId });
            var addr = rows.FirstOrDefault(r => Convert.ToInt32(r["id"]) == _addressId);
            if (addr != null)
            {
                cityTextBox.Text = addr["city"]?.ToString() ?? "";
                streetTextBox.Text = addr["street"]?.ToString() ?? "";
                buildingTextBox.Text = addr["building"]?.ToString() ?? "";
                apartmentTextBox.Text = addr["apartment"]?.ToString() ?? "";
                latitudeNumeric.Value = Convert.ToDecimal(addr["latitude"] ?? 59.9343m);
                longitudeNumeric.Value = Convert.ToDecimal(addr["longitude"] ?? 30.3351m);
                defaultCheckBox.Checked = Convert.ToBoolean(addr["is_default"]);
                activeCheckBox.Checked = Convert.ToBoolean(addr["active"]);

                var warehouseId = Convert.ToInt32(addr["warehouse_id"]);
                var wh = _warehouses.FirstOrDefault(w => w.Id == warehouseId);
                if (wh != null) warehouseComboBox.SelectedItem = wh.Name;
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка загрузки адреса", ex.Message);
        }
    }

    public void SetClientId(int clientId)
    {
        _clientId = clientId;
    }

    private async Task<bool> SaveAddress()
    {
        if (_repo == null || _clientId <= 0)
        {
            _notifications.ShowError("Ошибка", "Не указан клиент");
            return false;
        }

        if (string.IsNullOrWhiteSpace(cityTextBox.Text) || 
            string.IsNullOrWhiteSpace(streetTextBox.Text) || 
            string.IsNullOrWhiteSpace(buildingTextBox.Text))
        {
            _notifications.ShowError("Ошибка", "Заполните обязательные поля: город, улица, дом");
            return false;
        }

        if (warehouseComboBox.SelectedItem == null)
        {
            _notifications.ShowError("Ошибка", "Выберите склад");
            return false;
        }

        var warehouseName = warehouseComboBox.SelectedItem.ToString();
        var warehouseId = _warehouses.FirstOrDefault(w => w.Name == warehouseName)?.Id ?? 0;
        if (warehouseId == 0)
        {
            _notifications.ShowError("Ошибка", "Некорректный склад");
            return false;
        }

        try
        {
            if (_addressId > 0)
            {
                await _repo.ExecuteAsync("sp_client_address_update", new
                {
                    p_address_id = _addressId,
                    p_client_id = _clientId,
                    p_warehouse_id = warehouseId,
                    p_city = cityTextBox.Text.Trim(),
                    p_street = streetTextBox.Text.Trim(),
                    p_building = buildingTextBox.Text.Trim(),
                    p_apartment = apartmentTextBox.Text.Trim(),
                    p_latitude = latitudeNumeric.Value,
                    p_longitude = longitudeNumeric.Value,
                    p_is_default = defaultCheckBox.Checked ? 1 : 0,
                    p_active = activeCheckBox.Checked ? 1 : 0
                });
                _notifications.ShowSuccess("Адрес", "Адрес успешно обновлён");
            }
            else
            {
                await _repo.ExecuteAsync("sp_client_address_add", new
                {
                    p_client_id = _clientId,
                    p_warehouse_id = warehouseId,
                    p_city = cityTextBox.Text.Trim(),
                    p_street = streetTextBox.Text.Trim(),
                    p_building = buildingTextBox.Text.Trim(),
                    p_apartment = apartmentTextBox.Text.Trim(),
                    p_latitude = latitudeNumeric.Value,
                    p_longitude = longitudeNumeric.Value,
                    p_is_default = defaultCheckBox.Checked ? 1 : 0,
                    p_active = activeCheckBox.Checked ? 1 : 0
                });
                _notifications.ShowSuccess("Адрес", "Адрес успешно добавлен");
            }
            return true;
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Ошибка сохранения", ex.Message);
            return false;
        }
    }

    protected override async Task<bool> ValidateAndSaveAsync() => await SaveAddress();

    private void mapButton_Click(object? sender, EventArgs e)
    {
        using var form = new MapViewForm();
        form.ShowDialog(this);
    }
}

internal record WarehouseInfo(int Id, string Name);