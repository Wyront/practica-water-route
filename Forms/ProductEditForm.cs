namespace WaterRoute.Forms;

public partial class ProductEditForm : BaseEditForm
{
    public ProductEditForm() : this(null) { }

    public ProductEditForm(string[]? row) : base(
        row == null ? "Новый товар" : "Редактирование товара",
        "Цена, тип тары и условные места для проверки вместимости", 680, 610)
    {
        InitializeComponent();
        if (row != null)
        {
            // TODO: заполнить поля из row
        }
    }
}
