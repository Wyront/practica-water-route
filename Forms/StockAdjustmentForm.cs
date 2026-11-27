using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed class StockAdjustmentForm : BaseEditForm
{
    public StockAdjustmentForm() : base("Корректировка остатка", "Установка физического остатка с записью причины", 650, 490)
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        var table = FormLayout.CreateTwoColumnLayout();
        FormLayout.AddRow(table, "Склад *", FormLayout.CreateComboBox("Центральный склад", "Северный склад", "Приморский склад"));
        FormLayout.AddRow(table, "Товар *", FormLayout.CreateComboBox("Вода питьевая 19 л (WT-019)", "Вода питьевая 5 л (WT-005)", "Помпа механическая (AC-001)"));
        FormLayout.AddRow(table, "Текущий остаток", UITheme.CreateTextBox("84"));
        FormLayout.AddRow(table, "Новый остаток *", FormLayout.CreateNumeric(0, 0, 100000));
        FormLayout.AddRow(table, "Причина *", UITheme.CreateTextBox("Инвентаризация, повреждение, списание просроченного", true), 86);
        Content.Controls.Add(table);

        var warning = UITheme.CreateLabel(
            "Нельзя установить остаток ниже зарезервированного количества. Разница запишется в журнал как adjustment_in/out.",
            9F, FontStyle.Regular, UITheme.Warning, false);
        warning.Dock = DockStyle.Top;
        warning.Height = 70;
        Content.Controls.Add(warning);
        Content.Controls.SetChildIndex(table, 0);
        Content.Controls.SetChildIndex(warning, 1);
    }
}