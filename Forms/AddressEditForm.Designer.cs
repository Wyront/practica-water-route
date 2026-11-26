using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class AddressEditForm
{
    private TableLayoutPanel fields = null!;
    private TextBox cityTextBox = null!;
    private TextBox streetTextBox = null!;
    private TextBox buildingTextBox = null!;
    private TextBox apartmentTextBox = null!;
    private ComboBox warehouseComboBox = null!;
    private NumericUpDown latitudeNumeric = null!;
    private NumericUpDown longitudeNumeric = null!;
    private CheckBox defaultCheckBox = null!;
    private CheckBox activeCheckBox = null!;
    private Button mapButton = null!;
    private Label noteLabel = null!;

    private void InitializeComponent()
    {
        fields = FormLayout.CreateTwoColumnLayout();
        cityTextBox = UITheme.CreateTextBox();
        streetTextBox = UITheme.CreateTextBox();
        buildingTextBox = UITheme.CreateTextBox();
        apartmentTextBox = UITheme.CreateTextBox();
        warehouseComboBox = FormLayout.CreateComboBox(
            "Центральный склад", "Северный склад", "Приморский склад");
        latitudeNumeric = FormLayout.CreateNumeric(59.9343m, 59.5m, 60.3m, 6);
        longitudeNumeric = FormLayout.CreateNumeric(30.3351m, 29.5m, 31.0m, 6);
        defaultCheckBox = new CheckBox();
        activeCheckBox = new CheckBox();
        mapButton = UITheme.CreateButton("Указать на карте", true);
        noteLabel = UITheme.CreateLabel(string.Empty, 9F, FontStyle.Regular, UITheme.TextMuted, false);
        ((System.ComponentModel.ISupportInitialize)latitudeNumeric).BeginInit();
        ((System.ComponentModel.ISupportInitialize)longitudeNumeric).BeginInit();
        fields.SuspendLayout();
        SuspendLayout();
        //
        // fields
        //
        cityTextBox.Text = "Санкт-Петербург";
        FormLayout.AddRow(fields, "Город *", cityTextBox, 52);
        FormLayout.AddRow(fields, "Улица *", streetTextBox, 52);
        FormLayout.AddRow(fields, "Дом *", buildingTextBox, 52);
        FormLayout.AddRow(fields, "Квартира / офис", apartmentTextBox, 52);
        FormLayout.AddRow(fields, "Склад *", warehouseComboBox, 52);
        FormLayout.AddRow(fields, "Широта", latitudeNumeric, 52);
        FormLayout.AddRow(fields, "Долгота", longitudeNumeric, 52);
        defaultCheckBox.AutoSize = true;
        defaultCheckBox.Name = "defaultCheckBox";
        defaultCheckBox.Text = "Использовать по умолчанию";
        defaultCheckBox.UseVisualStyleBackColor = true;
        FormLayout.AddRow(fields, "Основной адрес", defaultCheckBox, 52);
        activeCheckBox.AutoSize = true;
        activeCheckBox.Checked = true;
        activeCheckBox.CheckState = CheckState.Checked;
        activeCheckBox.Name = "activeCheckBox";
        activeCheckBox.Text = "Адрес активен";
        activeCheckBox.UseVisualStyleBackColor = true;
        FormLayout.AddRow(fields, "Активность", activeCheckBox, 52);
        //
        // mapButton
        //
        mapButton.Dock = DockStyle.Top;
        mapButton.Height = 42;
        mapButton.Name = "mapButton";
        mapButton.Click += mapButton_Click;
        //
        // noteLabel
        //
        noteLabel.Dock = DockStyle.Bottom;
        noteLabel.Height = 70;
        noteLabel.Name = "noteLabel";
        noteLabel.Text = "В исторических заказах сохраняется отдельная текстовая копия адреса.";
        //
        // AddressEditForm
        //
        Content.Controls.Add(noteLabel);
        Content.Controls.Add(mapButton);
        Content.Controls.Add(fields);
        Name = "AddressEditForm";
        ((System.ComponentModel.ISupportInitialize)latitudeNumeric).EndInit();
        ((System.ComponentModel.ISupportInitialize)longitudeNumeric).EndInit();
        fields.ResumeLayout(false);
        fields.PerformLayout();
        ResumeLayout(false);
    }
}
