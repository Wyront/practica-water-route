namespace WaterRoute.Forms;

public partial class AddressEditForm : BaseEditForm
{
    public AddressEditForm() : this(null) { }

    public AddressEditForm(string[]? row) : base(
        row == null ? "Новый адрес" : "Редактирование адреса",
        "Адрес привязывается к складу и точке на карте Санкт-Петербурга", 760, 650)
    {
        InitializeComponent();
        if (row != null)
        {
            // TODO: заполнить поля из row
        }
    }

    private void mapButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("Здесь будет открыт картографический компонент Mapsui с ручной установкой маркера.",
            "Макет карты", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
