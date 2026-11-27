using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class ProductEditForm
{
    private TableLayoutPanel fields = null!;
    private TextBox skuTextBox = null!;
    private TextBox nameTextBox = null!;
    private ComboBox typeComboBox = null!;
    private NumericUpDown priceNumeric = null!;
    private NumericUpDown capacityNumeric = null!;
    private CheckBox returnableCheckBox = null!;
    private CheckBox activeCheckBox = null!;
    private Label noteLabel = null!;

    private void InitializeComponent()
    {
        fields = FormLayout.CreateTwoColumnLayout();
        skuTextBox = UITheme.CreateTextBox();
        nameTextBox = UITheme.CreateTextBox();
        typeComboBox = FormLayout.CreateComboBox("Бутыль 19 л", "ПЭТ 5 л", "Без тары");
        priceNumeric = new NumericUpDown();
        capacityNumeric = FormLayout.CreateNumeric(2, 1, 1000);
        returnableCheckBox = new CheckBox();
        activeCheckBox = new CheckBox();
        noteLabel = UITheme.CreateLabel(string.Empty, 9F, FontStyle.Regular, UITheme.TextMuted, false);
        ((System.ComponentModel.ISupportInitialize)priceNumeric).BeginInit();
        ((System.ComponentModel.ISupportInitialize)capacityNumeric).BeginInit();
        fields.SuspendLayout();
        SuspendLayout();
        //
        // fields
        //
        skuTextBox.Text = "WT-019";
        FormLayout.AddRow(fields, "Артикул *", skuTextBox, 52);
        nameTextBox.Text = "Вода питьевая 19 л";
        FormLayout.AddRow(fields, "Наименование *", nameTextBox, 52);
        FormLayout.AddRow(fields, "Тип воды / тары *", typeComboBox, 52);
        priceNumeric.Font = UITheme.BodyFont();
        priceNumeric.Maximum = 100000000;
        priceNumeric.Name = "priceNumeric";
        priceNumeric.ThousandsSeparator = true;
        priceNumeric.Value = 25000;
        FormLayout.AddRow(fields, "Цена, копейки *", priceNumeric, 52);
        FormLayout.AddRow(fields, "Условных мест *", capacityNumeric, 52);
        returnableCheckBox.AutoSize = true;
        returnableCheckBox.Checked = true;
        returnableCheckBox.CheckState = CheckState.Checked;
        returnableCheckBox.Name = "returnableCheckBox";
        returnableCheckBox.Text = "Товар создаёт обязательство по таре";
        returnableCheckBox.UseVisualStyleBackColor = true;
        FormLayout.AddRow(fields, "Возвратная тара", returnableCheckBox, 52);
        activeCheckBox.AutoSize = true;
        activeCheckBox.Checked = true;
        activeCheckBox.CheckState = CheckState.Checked;
        activeCheckBox.Name = "activeCheckBox";
        activeCheckBox.Text = "Товар доступен для продажи";
        activeCheckBox.UseVisualStyleBackColor = true;
        FormLayout.AddRow(fields, "Активность", activeCheckBox, 52);
        //
        // noteLabel
        //
        noteLabel.Dock = DockStyle.Bottom;
        noteLabel.Height = 70;
        noteLabel.Name = "noteLabel";
        noteLabel.Text = "Цена хранится в копейках. В позиции заказа сохраняется отдельная историческая цена.";
        //
        // ProductEditForm
        //
        Content.Controls.Add(noteLabel);
        Content.Controls.Add(fields);
        Name = "ProductEditForm";
        ((System.ComponentModel.ISupportInitialize)priceNumeric).EndInit();
        ((System.ComponentModel.ISupportInitialize)capacityNumeric).EndInit();
        fields.ResumeLayout(false);
        fields.PerformLayout();
        ResumeLayout(false);
    }
}
