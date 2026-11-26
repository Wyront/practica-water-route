using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed partial class CourierProblemsForm : BaseEditForm
{
    public CourierProblemsForm() : base("Мои проблемы", "Регистрация и отслеживание проблем доставки", 1000, 700)
    {
        InitializeComponent();
    }

    private void createButton_Click(object? sender, EventArgs e)
    {
        using var form = new ProblemEditForm(Models.UserRole.Courier);
        form.ShowDialog(this);
    }

    private void RefreshData()
    {
        // TODO: sp_problem_list с фильтром по курьеру
    }
}