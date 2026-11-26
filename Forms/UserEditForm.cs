namespace WaterRoute.Forms;

public partial class UserEditForm : BaseEditForm
{
    public UserEditForm() : this(null) { }

    public UserEditForm(string[]? row) : base(
        row == null ? "Новый пользователь" : "Редактирование пользователя",
        "Учётная запись, роль, контактные данные и активность", 650, 570)
    {
        InitializeComponent();
        if (row != null)
        {
            // TODO: заполнить поля из row
        }
    }

    public UserEditForm(bool readOnly) : this(null)
    {
        // TODO: настроить read-only
    }
}
