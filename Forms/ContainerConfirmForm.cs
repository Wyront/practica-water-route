using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed class ContainerConfirmForm : BaseEditForm
{
    public ContainerConfirmForm(string[] row) : base($"Подтверждение приёмки тары: {row[1]}", $"Маршрут {row[0]}, тип тары {row[2]}", 600, 480)
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        var table = FormLayout.CreateTwoColumnLayout();
        FormLayout.AddRow(table, "Маршрут", UITheme.CreateTextBox("R-020"));
        FormLayout.AddRow(table, "Клиент", UITheme.CreateTextBox("Анна Воронова"));
        FormLayout.AddRow(table, "Тип тары", UITheme.CreateTextBox("Бутыль 19 л"));
        FormLayout.AddRow(table, "Заявлено курьером", UITheme.CreateTextBox("2"));
        FormLayout.AddRow(table, "Фактически принято *", FormLayout.CreateNumeric(0, 0, 1000));
        FormLayout.AddRow(table, "Комментарий", UITheme.CreateTextBox("Состояние тары, повреждения", true), 70);
        Content.Controls.Add(table);

        var warning = UITheme.CreateLabel(
            "Нельзя подтвердить больше, чем заявлено курьером. После подтверждения долг клиента уменьшится.",
            9F, FontStyle.Regular, UITheme.Warning, false);
        warning.Dock = DockStyle.Top;
        warning.Height = 70;
        Content.Controls.Add(warning);
        Content.Controls.SetChildIndex(table, 0);
        Content.Controls.SetChildIndex(warning, 1);
    }
}