using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class StockMovementForm
{
    private TableLayoutPanel fields = null!;
    private ComboBox warehouseComboBox = null!;
    private ComboBox productComboBox = null!;
    private ComboBox kindComboBox = null!;
    private NumericUpDown quantityNumeric = null!;
    private TextBox currentStockTextBox = null!;
    private TextBox commentTextBox = null!;
    private Label warningLabel = null!;

    private void InitializeComponent()
    {
        fields = FormLayout.CreateTwoColumnLayout();
        warehouseComboBox = FormLayout.CreateComboBox(
            "Центральный склад", "Северный склад", "Приморский склад");
        productComboBox = FormLayout.CreateComboBox(
            "Вода питьевая 19 л", "Вода питьевая 5 л", "Пустая бутыль 19 л");
        kindComboBox = FormLayout.CreateComboBox("Поступление", "Списание", "Корректировка");
        quantityNumeric = FormLayout.CreateNumeric(1, 1, 100000);
        currentStockTextBox = UITheme.CreateTextBox();
        commentTextBox = UITheme.CreateTextBox(string.Empty, true);
        warningLabel = UITheme.CreateLabel(string.Empty, 9F, FontStyle.Regular, UITheme.Warning, false);
        ((System.ComponentModel.ISupportInitialize)quantityNumeric).BeginInit();
        fields.SuspendLayout();
        SuspendLayout();
        //
        // fields
        //
        FormLayout.AddRow(fields, "Склад *", warehouseComboBox, 52);
        FormLayout.AddRow(fields, "Товар *", productComboBox, 52);
        FormLayout.AddRow(fields, "Тип операции *", kindComboBox, 52);
        FormLayout.AddRow(fields, "Количество *", quantityNumeric, 52);
        currentStockTextBox.ReadOnly = true;
        currentStockTextBox.Text = "84";
        FormLayout.AddRow(fields, "Текущий остаток", currentStockTextBox, 52);
        FormLayout.AddRow(fields, "Комментарий *", commentTextBox, 110);
        //
        // warningLabel
        //
        warningLabel.Dock = DockStyle.Bottom;
        warningLabel.Height = 70;
        warningLabel.Name = "warningLabel";
        warningLabel.Text = "Операция не сможет создать отрицательный остаток. При списании дополнительно проверяется резерв.";
        //
        // StockMovementForm
        //
        Content.Controls.Add(warningLabel);
        Content.Controls.Add(fields);
        Name = "StockMovementForm";
        ((System.ComponentModel.ISupportInitialize)quantityNumeric).EndInit();
        fields.ResumeLayout(false);
        fields.PerformLayout();
        ResumeLayout(false);
    }
}
