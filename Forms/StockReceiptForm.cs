using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed class StockReceiptForm : BaseEditForm
{
    public StockReceiptForm() : base("Поступление товара", "Остаток и запись в журнале движений сохраняются одной транзакцией", 650, 520)
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        var table = FormLayout.CreateTwoColumnLayout();
        FormLayout.AddRow(table, "Склад *", FormLayout.CreateComboBox("Центральный склад", "Северный склад", "Приморский склад"));
        FormLayout.AddRow(table, "Товар *", FormLayout.CreateComboBox("Вода питьевая 19 л (WT-019)", "Вода питьевая 5 л (WT-005)", "Помпа механическая (AC-001)"));
        FormLayout.AddRow(table, "Количество *", FormLayout.CreateNumeric(1, 1, 100000));
        FormLayout.AddRow(table, "Комментарий *", UITheme.CreateTextBox("Поставка, возврат от клиента или иное", true), 86);
        Content.Controls.Add(table);

        var warning = UITheme.CreateLabel(
            "Операция не сможет создать отрицательный остаток. Запись в журнале движений и обновление остатка — в одной транзакции.",
            9F, FontStyle.Regular, UITheme.Warning, false);
        warning.Dock = DockStyle.Top;
        warning.Height = 70;
        Content.Controls.Add(warning);
        Content.Controls.SetChildIndex(table, 0);
        Content.Controls.SetChildIndex(warning, 1);
    }
}