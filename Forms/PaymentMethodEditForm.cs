using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed class PaymentMethodEditForm : BaseEditForm
{
    public PaymentMethodEditForm() : this(null) { }

    public PaymentMethodEditForm(string[]? row) : base(
        row == null ? "Новый способ оплаты" : "Редактирование способа оплаты",
        "Код используется в заказах, название — для отображения клиентам", 550, 420)
    {
        var table = FormLayout.CreateTwoColumnLayout();
        var codeBox = UITheme.CreateTextBox("cash");
        var nameBox = UITheme.CreateTextBox("Наличные");
        FormLayout.AddRow(table, "Код *", codeBox);
        FormLayout.AddRow(table, "Название *", nameBox);
        FormLayout.AddRow(table, "Активен", new CheckBox { Text = "Доступен для выбора", Checked = true, AutoSize = true });
        Content.Controls.Add(table);

        if (row != null)
        {
            codeBox.Text = row[0];
            nameBox.Text = row[1];
            codeBox.ReadOnly = true; // код не меняется
        }
    }
}