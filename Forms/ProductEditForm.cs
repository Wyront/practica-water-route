using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public partial class ProductEditForm : BaseEditForm
{
    private sealed record WaterTypeItem(int Id, string Name)
    {
        public override string ToString() => Name;
    }

    private readonly IRepository? _repo;
    private readonly IDictionary<string, object?>? _data;

    public ProductEditForm() : this(null, null) { }

    public ProductEditForm(IRepository? repo, IDictionary<string, object?>? data) : base(
        data == null ? "Новый товар" : "Редактирование товара",
        "Цена, тип тары и условные места для проверки вместимости", 680, 610)
    {
        _repo = repo;
        _data = data;
        InitializeComponent();

        if (data != null)
        {
            skuTextBox.Text = Convert.ToString(data["sku"]) ?? "";
            nameTextBox.Text = Convert.ToString(data["name"]) ?? "";
            priceNumeric.Value = Convert.ToDecimal(data["price_kopecks"]);
            capacityNumeric.Value = Convert.ToDecimal(data["capacity_units"]);
            returnableCheckBox.Checked = Convert.ToBoolean(data["returnable"]);
            activeCheckBox.Checked = Convert.ToBoolean(data["active"]);
        }
        else if (repo != null)
        {
            skuTextBox.Clear();
            nameTextBox.Clear();
        }

        Load += async (_, _) => await LoadWaterTypesAsync();
    }

    private async Task LoadWaterTypesAsync()
    {
        if (_repo == null) return;
        try
        {
            var rows = await _repo.QueryAsync<dynamic>("sp_directory_list", new { type = "water_type" });
            typeComboBox.Items.Clear();
            foreach (IDictionary<string, object?> r in rows)
            {
                // В новый товар предлагаем только активные типы; у существующего показываем и его текущий тип.
                var id = Convert.ToInt32(r["id"]);
                var isCurrent = _data != null && Convert.ToInt32(_data["water_type_id"]) == id;
                if (Convert.ToBoolean(r["active"]) || isCurrent)
                    typeComboBox.Items.Add(new WaterTypeItem(id, Convert.ToString(r["name"]) ?? ""));
            }

            if (_data != null)
            {
                var currentId = Convert.ToInt32(_data["water_type_id"]);
                typeComboBox.SelectedItem = typeComboBox.Items.Cast<WaterTypeItem>().FirstOrDefault(i => i.Id == currentId);
            }
            else if (typeComboBox.Items.Count > 0)
            {
                typeComboBox.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            UiErrors.Show(ex, this);
        }
    }

    protected override async Task<bool> ValidateAndSaveAsync()
    {
        if (_repo == null) { UiErrors.Warn("Демо-режим: нет подключения к базе данных.", this); return false; }

        var sku = skuTextBox.Text.Trim();
        var name = nameTextBox.Text.Trim();
        if (sku.Length == 0 || name.Length == 0) { UiErrors.Warn("Заполните артикул и наименование.", this); return false; }
        if (typeComboBox.SelectedItem is not WaterTypeItem type) { UiErrors.Warn("Выберите тип воды / тары.", this); return false; }
        if (priceNumeric.Value < 0) { UiErrors.Warn("Цена не может быть отрицательной.", this); return false; }

        await _repo.ExecuteAsync("sp_product_crud", new
        {
            action = _data == null ? "CREATE" : "UPDATE",
            id = _data == null ? 0 : Convert.ToInt32(_data["id"]),
            sku,
            name,
            water_type_id = type.Id,
            price_kopecks = (int)priceNumeric.Value,
            returnable = (sbyte)(returnableCheckBox.Checked ? 1 : 0),
            capacity_units = (int)capacityNumeric.Value,
            active = (sbyte)(activeCheckBox.Checked ? 1 : 0)
        });
        return true;
    }
}
