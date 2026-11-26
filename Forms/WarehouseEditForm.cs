namespace WaterRoute.Forms;

public partial class WarehouseEditForm : BaseEditForm
{
    public WarehouseEditForm() : this(null) { }

    public WarehouseEditForm(string[]? row) : base(
        row == null ? "Новый склад" : "Редактирование склада",
        "Склад обслуживает адреса и является начальной точкой маршрута", 680, 520)
    {
        InitializeComponent();
        if (row != null)
        {
            // TODO: заполнить поля из row
        }
    }

    private void mapButton_Click(object? sender, EventArgs e) =>
        MessageBox.Show("Здесь будет открыт Mapsui для установки координат склада.", "Макет карты");
}
