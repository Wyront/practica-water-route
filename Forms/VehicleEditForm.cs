namespace WaterRoute.Forms;

public partial class VehicleEditForm : BaseEditForm
{
    public VehicleEditForm() : this(null) { }

    public VehicleEditForm(string[]? row) : base(
        row == null ? "Новый автомобиль" : "Редактирование автомобиля",
        "Транспорт назначается только на один активный маршрут", 650, 490)
    {
        InitializeComponent();
        if (row != null)
        {
            // TODO: заполнить поля из row
        }
    }
}
