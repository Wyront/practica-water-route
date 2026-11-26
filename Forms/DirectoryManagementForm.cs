using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed partial class DirectoryManagementForm : BaseEditForm
{
    public DirectoryManagementForm() : base("Справочники", "Администрирование способов оплаты, типов воды, товаров, складов и автомобилей", 1100, 750)
    {
        InitializeComponent();
    }

    private void createButton_Click(object? sender, EventArgs e)
    {
        var page = _tabs.SelectedTab;
        if (page.Tag is string type)
        {
            Form? form = type switch
            {
                "payment" => new PaymentMethodEditForm(),
                "water_type" => new WaterTypeEditForm(),
                "product" => new ProductEditForm(),
                "warehouse" => new WarehouseEditForm(),
                "vehicle" => new VehicleEditForm(),
                _ => null
            };
            if (form != null && form.ShowDialog(this) == DialogResult.OK)
                RefreshCurrentTab();
        }
    }

    private void editButton_Click(object? sender, EventArgs e)
    {
        var grid = GetCurrentGrid();
        if (grid?.SelectedRows.Count == 0) return;
        var row = (string[])grid.SelectedRows[0].Tag!;
        var type = _tabs.SelectedTab.Tag as string;
        Form? form = type switch
        {
            "payment" => new PaymentMethodEditForm(row),
            "water_type" => new WaterTypeEditForm(row),
            "product" => new ProductEditForm(row),
            "warehouse" => new WarehouseEditForm(row),
            "vehicle" => new VehicleEditForm(row),
            _ => null
        };
        if (form != null && form.ShowDialog(this) == DialogResult.OK)
            RefreshCurrentTab();
    }

    private void deactivateButton_Click(object? sender, EventArgs e)
    {
        var grid = GetCurrentGrid();
        if (grid?.SelectedRows.Count == 0) return;
        if (MessageBox.Show("Деактивировать выбранную запись?", "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            // TODO: соответствующая процедура деактивации
            MessageBox.Show("Функция будет подключена после интеграции с БД", "WaterRoute");
        }
    }

    private void RefreshData()
    {
        // TODO: загрузка данных для всех вкладок
    }

    private void RefreshCurrentTab()
    {
        // TODO: перезагрузка текущей вкладки
    }

    private DataGridView? GetCurrentGrid()
    {
        return _tabs.SelectedTab?.Controls.OfType<DataGridView>().FirstOrDefault();
    }
}