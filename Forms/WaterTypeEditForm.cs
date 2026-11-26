using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed class WaterTypeEditForm : BaseEditForm
{
    public WaterTypeEditForm() : this(null) { }

    public WaterTypeEditForm(string[]? row) : base(
        row == null ? "Новый тип воды/тары" : "Редактирование типа воды/тары",
        "Название и объём определяют тип тары для учёта возврата", 550, 420)
    {
        var table = FormLayout.CreateTwoColumnLayout();
        var nameBox = UITheme.CreateTextBox("Бутыль 19 л");
        var volumeBox = FormLayout.CreateNumeric(19, 0, 100, 2);
        FormLayout.AddRow(table, "Название *", nameBox);
        FormLayout.AddRow(table, "Объём, л *", volumeBox);
        FormLayout.AddRow(table, "Активен", new CheckBox { Text = "Доступен для товаров", Checked = true, AutoSize = true });
        Content.Controls.Add(table);

        if (row != null)
        {
            nameBox.Text = row[0];
            volumeBox.Value = decimal.Parse(row[1]);
        }
    }
}