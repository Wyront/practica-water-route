using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class WarehouseEditForm
{
    private TableLayoutPanel fields = null!;
    private TextBox nameTextBox = null!;
    private TextBox addressTextBox = null!;
    private NumericUpDown latitudeNumeric = null!;
    private NumericUpDown longitudeNumeric = null!;
    private CheckBox activeCheckBox = null!;
    private Button mapButton = null!;

    private void InitializeComponent()
    {
        fields = FormLayout.CreateTwoColumnLayout();
        nameTextBox = UITheme.CreateTextBox();
        addressTextBox = UITheme.CreateTextBox();
        latitudeNumeric = FormLayout.CreateNumeric(59.9343m, 59.5m, 60.3m, 6);
        longitudeNumeric = FormLayout.CreateNumeric(30.3351m, 29.5m, 31.0m, 6);
        activeCheckBox = new CheckBox();
        mapButton = UITheme.CreateButton("Указать склад на карте", true);
        ((System.ComponentModel.ISupportInitialize)latitudeNumeric).BeginInit();
        ((System.ComponentModel.ISupportInitialize)longitudeNumeric).BeginInit();
        fields.SuspendLayout();
        SuspendLayout();
        //
        // fields
        //
        nameTextBox.Text = "Центральный склад";
        FormLayout.AddRow(fields, "Название *", nameTextBox, 56);
        addressTextBox.Text = "Санкт-Петербург, Складская улица, 1";
        FormLayout.AddRow(fields, "Адрес *", addressTextBox, 56);
        FormLayout.AddRow(fields, "Широта", latitudeNumeric, 56);
        FormLayout.AddRow(fields, "Долгота", longitudeNumeric, 56);
        activeCheckBox.AutoSize = true;
        activeCheckBox.Checked = true;
        activeCheckBox.CheckState = CheckState.Checked;
        activeCheckBox.Name = "activeCheckBox";
        activeCheckBox.Text = "Склад активен";
        activeCheckBox.UseVisualStyleBackColor = true;
        FormLayout.AddRow(fields, "Активность", activeCheckBox, 56);
        //
        // mapButton
        //
        mapButton.Dock = DockStyle.Top;
        mapButton.Height = 42;
        mapButton.Name = "mapButton";
        mapButton.Click += mapButton_Click;
        //
        // WarehouseEditForm
        //
        Content.Controls.Add(mapButton);
        Content.Controls.Add(fields);
        Name = "WarehouseEditForm";
        ((System.ComponentModel.ISupportInitialize)latitudeNumeric).EndInit();
        ((System.ComponentModel.ISupportInitialize)longitudeNumeric).EndInit();
        fields.ResumeLayout(false);
        fields.PerformLayout();
        ResumeLayout(false);
    }
}
