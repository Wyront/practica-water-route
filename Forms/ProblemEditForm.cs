using WaterRoute.Models;

namespace WaterRoute.Forms;

public partial class ProblemEditForm : BaseEditForm
{
    public ProblemEditForm()
        : base("Регистрация проблемы", "Проблема связывается с маршрутом, заказом или доставкой", 760, 680)
    {
        InitializeComponent();
    }

    public ProblemEditForm(UserRole role, bool readOnly = false)
        : base("Регистрация проблемы", "Проблема связывается с маршрутом, заказом или доставкой", 760, 680)
    {
        InitializeComponent(role, readOnly);
    }
}
