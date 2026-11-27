using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed partial class DirectoryManagementForm : BaseEditForm
{
    private readonly IRepository? _repo;
    private readonly Dictionary<string, DataGridView> _grids = new();

    public DirectoryManagementForm() : this(null) { }

    public DirectoryManagementForm(IRepository? repo)
        : base("Справочники", "Администрирование способов оплаты, типов воды, товаров, складов и автомобилей", 1100, 750)
    {
        _repo = repo;
        InitializeComponent();
        _tabs.SelectedIndexChanged += (_, _) => RefreshCurrentTab();
        Load += (_, _) => RefreshCurrentTab();
    }

    private string? CurrentType => _tabs.SelectedTab?.Tag as string;

    private DataGridView? GetCurrentGrid() =>
        CurrentType != null && _grids.TryGetValue(CurrentType, out var grid) ? grid : null;

    private IDictionary<string, object?>? SelectedData() =>
        GetCurrentGrid() is { SelectedRows.Count: > 0 } g ? g.SelectedRows[0].Tag as IDictionary<string, object?> : null;

    private Form? CreateEditForm(string type, IDictionary<string, object?>? data) => type switch
    {
        "payment" => new PaymentMethodEditForm(_repo, data),
        "water_type" => new WaterTypeEditForm(_repo, data),
        "product" => new ProductEditForm(_repo, data),
        "warehouse" => new WarehouseEditForm(_repo, data),
        "vehicle" => new VehicleEditForm(_repo, data),
        _ => null
    };

    private void createButton_Click(object? sender, EventArgs e)
    {
        if (CurrentType is not { } type) return;
        using var form = CreateEditForm(type, null);
        if (form != null && form.ShowDialog(this) == DialogResult.OK)
            RefreshCurrentTab();
    }

    private void editButton_Click(object? sender, EventArgs e)
    {
        if (CurrentType is not { } type) return;
        var data = SelectedData();
        if (data == null) { UiErrors.Info("Выберите запись в таблице.", this); return; }
        using var form = CreateEditForm(type, data);
        if (form != null && form.ShowDialog(this) == DialogResult.OK)
            RefreshCurrentTab();
    }

    /// <summary>Деактивирует выбранную запись, а если она уже неактивна — возвращает в работу.</summary>
    private async void deactivateButton_Click(object? sender, EventArgs e)
    {
        if (CurrentType is not { } type) return;
        var data = SelectedData();
        if (data == null) { UiErrors.Info("Выберите запись в таблице.", this); return; }
        if (_repo == null) { UiErrors.Info("Демо-режим: нет подключения к БД.", this); return; }

        var wasActive = Convert.ToBoolean(data["active"]);
        var question = wasActive ? "Деактивировать выбранную запись?" : "Активировать выбранную запись?";
        if (MessageBox.Show(this, question, "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            var active = (sbyte)(wasActive ? 0 : 1);
            var id = Convert.ToInt32(data["id"]);
            switch (type)
            {
                case "payment":
                    await _repo.ExecuteAsync("sp_payment_method_crud", new
                    {
                        action = "UPDATE", id, code = data["code"], name = data["name"], active
                    });
                    break;
                case "water_type":
                    await _repo.ExecuteAsync("sp_water_type_crud", new
                    {
                        action = "UPDATE", id, name = data["name"], volume_l = Convert.ToDecimal(data["volume_l"]), active
                    });
                    break;
                case "product":
                    await _repo.ExecuteAsync("sp_product_crud", new
                    {
                        action = "UPDATE", id, sku = data["sku"], name = data["name"],
                        water_type_id = Convert.ToInt32(data["water_type_id"]),
                        price_kopecks = Convert.ToInt32(data["price_kopecks"]),
                        returnable = (sbyte)(Convert.ToBoolean(data["returnable"]) ? 1 : 0),
                        capacity_units = Convert.ToInt32(data["capacity_units"]), active
                    });
                    break;
                case "warehouse":
                    await _repo.ExecuteAsync("sp_warehouse_crud", new
                    {
                        action = "UPDATE", id, name = data["name"], address = data["address"],
                        latitude = data["latitude"], longitude = data["longitude"], active
                    });
                    break;
                case "vehicle":
                    await _repo.ExecuteAsync("sp_vehicle_crud", new
                    {
                        action = "UPDATE", id, plate = data["plate"], model = data["model"],
                        capacity_units = Convert.ToInt32(data["capacity_units"]), active
                    });
                    break;
            }
            RefreshCurrentTab();
        }
        catch (Exception ex)
        {
            UiErrors.Show(ex, this);
        }
    }

    private void RefreshData() => RefreshCurrentTab();

    private async void RefreshCurrentTab()
    {
        if (_repo == null || CurrentType is not { } type || GetCurrentGrid() is not { } grid) return;
        try
        {
            var rows = await _repo.QueryAsync<dynamic>("sp_directory_list", new { type });
            var selectedId = SelectedData() is { } cur ? Convert.ToInt32(cur["id"]) : (int?)null;

            grid.Rows.Clear();
            foreach (IDictionary<string, object?> r in rows)
            {
                var active = Convert.ToBoolean(r["active"]) ? "Да" : "Нет";
                string[] cells = type switch
                {
                    "payment" => [Str(r, "name"), active],
                    "water_type" => [Str(r, "name"), Convert.ToDecimal(r["volume_l"]).ToString("0.##"), active],
                    "product" => [Str(r, "sku"), Str(r, "name"), Str(r, "water_type_name"),
                                  (Convert.ToInt32(r["price_kopecks"]) / 100m).ToString("N2") + " ₽",
                                  Str(r, "capacity_units"),
                                  Convert.ToBoolean(r["returnable"]) ? "Возвратная" : "Нет", active],
                    "warehouse" => [Str(r, "name"), Str(r, "address"),
                                    r["latitude"] == null || r["longitude"] == null ? "—" : $"{Convert.ToDecimal(r["latitude"]):0.0000}, {Convert.ToDecimal(r["longitude"]):0.0000}",
                                    active],
                    "vehicle" => [Str(r, "plate"), Str(r, "model"), Str(r, "capacity_units"), active],
                    _ => []
                };
                var index = grid.Rows.Add(cells);
                grid.Rows[index].Tag = r;
                if (active == "Нет")
                    grid.Rows[index].DefaultCellStyle.ForeColor = UITheme.TextMuted;
                if (selectedId != null && Convert.ToInt32(r["id"]) == selectedId)
                    grid.Rows[index].Selected = true;
            }
        }
        catch (Exception ex)
        {
            UiErrors.Show(ex, this);
        }
    }

    private static string Str(IDictionary<string, object?> row, string key) => Convert.ToString(row[key]) ?? "";

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.F5) { RefreshCurrentTab(); return true; }
        return base.ProcessCmdKey(ref msg, keyData);
    }
}
