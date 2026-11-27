using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public partial class WarehouseEditForm : BaseEditForm
{
    private readonly IRepository? _repo;
    private readonly IDictionary<string, object?>? _data;

    public WarehouseEditForm() : this(null, null) { }

    public WarehouseEditForm(IRepository? repo, IDictionary<string, object?>? data) : base(
        data == null ? "Новый склад" : "Редактирование склада",
        "Склад обслуживает адреса и является начальной точкой маршрута", 680, 520)
    {
        _repo = repo;
        _data = data;
        InitializeComponent();

        if (data != null)
        {
            nameTextBox.Text = Convert.ToString(data["name"]) ?? "";
            addressTextBox.Text = Convert.ToString(data["address"]) ?? "";
            if (data["latitude"] != null) latitudeNumeric.Value = Clamp(latitudeNumeric, Convert.ToDecimal(data["latitude"]));
            if (data["longitude"] != null) longitudeNumeric.Value = Clamp(longitudeNumeric, Convert.ToDecimal(data["longitude"]));
            activeCheckBox.Checked = Convert.ToBoolean(data["active"]);
        }
        else if (repo != null)
        {
            nameTextBox.Clear();
            addressTextBox.Clear();
        }
    }

    private static decimal Clamp(NumericUpDown box, decimal value) => Math.Min(box.Maximum, Math.Max(box.Minimum, value));

    protected override async Task<bool> ValidateAndSaveAsync()
    {
        if (_repo == null) { UiErrors.Warn("Демо-режим: нет подключения к базе данных.", this); return false; }

        var name = nameTextBox.Text.Trim();
        var address = addressTextBox.Text.Trim();
        if (name.Length == 0 || address.Length == 0) { UiErrors.Warn("Заполните название и адрес склада.", this); return false; }

        await _repo.ExecuteAsync("sp_warehouse_crud", new
        {
            action = _data == null ? "CREATE" : "UPDATE",
            id = _data == null ? 0 : Convert.ToInt32(_data["id"]),
            name,
            address,
            latitude = latitudeNumeric.Value,
            longitude = longitudeNumeric.Value,
            active = (sbyte)(activeCheckBox.Checked ? 1 : 0)
        });
        return true;
    }

    private void mapButton_Click(object? sender, EventArgs e) =>
        UiErrors.Info("Выбор точки на карте будет доступен в разделе карты. Пока введите широту и долготу вручную.", this);
}
