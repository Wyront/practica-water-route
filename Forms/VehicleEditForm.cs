using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public partial class VehicleEditForm : BaseEditForm
{
    private readonly IRepository? _repo;
    private readonly IDictionary<string, object?>? _data;

    public VehicleEditForm() : this(null, null) { }

    public VehicleEditForm(IRepository? repo, IDictionary<string, object?>? data) : base(
        data == null ? "Новый автомобиль" : "Редактирование автомобиля",
        "Транспорт назначается только на один активный маршрут", 650, 490)
    {
        _repo = repo;
        _data = data;
        InitializeComponent();

        if (data != null)
        {
            plateTextBox.Text = Convert.ToString(data["plate"]) ?? "";
            modelTextBox.Text = Convert.ToString(data["model"]) ?? "";
            capacityNumeric.Value = Convert.ToDecimal(data["capacity_units"]);
            activeCheckBox.Checked = Convert.ToBoolean(data["active"]);
        }
        else if (repo != null)
        {
            plateTextBox.Clear();
            modelTextBox.Clear();
        }
    }

    protected override async Task<bool> ValidateAndSaveAsync()
    {
        if (_repo == null) { UiErrors.Warn("Демо-режим: нет подключения к базе данных.", this); return false; }

        var plate = plateTextBox.Text.Trim().ToUpperInvariant();
        var model = modelTextBox.Text.Trim();
        if (plate.Length < 6 || model.Length == 0) { UiErrors.Warn("Укажите госномер (не короче 6 символов) и модель.", this); return false; }

        await _repo.ExecuteAsync("sp_vehicle_crud", new
        {
            action = _data == null ? "CREATE" : "UPDATE",
            id = _data == null ? 0 : Convert.ToInt32(_data["id"]),
            plate,
            model,
            capacity_units = (int)capacityNumeric.Value,
            active = (sbyte)(activeCheckBox.Checked ? 1 : 0)
        });
        return true;
    }
}
