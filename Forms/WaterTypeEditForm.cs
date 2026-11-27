using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed class WaterTypeEditForm : BaseEditForm
{
    private readonly IRepository? _repo;
    private readonly IDictionary<string, object?>? _data;
    private readonly TextBox _nameBox;
    private readonly NumericUpDown _volumeBox;
    private readonly CheckBox _activeBox;

    public WaterTypeEditForm() : this(null, null) { }

    public WaterTypeEditForm(IRepository? repo, IDictionary<string, object?>? data) : base(
        data == null ? "Новый тип воды/тары" : "Редактирование типа воды/тары",
        "Название и объём определяют тип тары для учёта возврата", 550, 420)
    {
        _repo = repo;
        _data = data;

        var table = FormLayout.CreateTwoColumnLayout();
        _nameBox = UITheme.CreateTextBox("Бутыль 19 л");
        _volumeBox = FormLayout.CreateNumeric(19, 0, 100, 2);
        _activeBox = new CheckBox { Text = "Доступен для товаров", Checked = true, AutoSize = true };
        FormLayout.AddRow(table, "Название *", _nameBox);
        FormLayout.AddRow(table, "Объём, л *", _volumeBox);
        FormLayout.AddRow(table, "Активен", _activeBox);
        Content.Controls.Add(table);

        if (data != null)
        {
            _nameBox.Text = Convert.ToString(data["name"]) ?? "";
            _volumeBox.Value = Convert.ToDecimal(data["volume_l"]);
            _activeBox.Checked = Convert.ToBoolean(data["active"]);
        }
    }

    protected override async Task<bool> ValidateAndSaveAsync()
    {
        if (_repo == null) { UiErrors.Warn("Демо-режим: нет подключения к базе данных.", this); return false; }

        var name = _nameBox.Text.Trim();
        if (name.Length == 0) { UiErrors.Warn("Введите название.", this); return false; }

        await _repo.ExecuteAsync("sp_water_type_crud", new
        {
            action = _data == null ? "CREATE" : "UPDATE",
            id = _data == null ? 0 : Convert.ToInt32(_data["id"]),
            name,
            volume_l = _volumeBox.Value,
            active = (sbyte)(_activeBox.Checked ? 1 : 0)
        });
        return true;
    }
}
