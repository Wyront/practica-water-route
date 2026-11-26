using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class VehicleEditForm
{
    private TableLayoutPanel fields = null!;
    private TextBox plateTextBox = null!;
    private TextBox modelTextBox = null!;
    private NumericUpDown capacityNumeric = null!;
    private CheckBox activeCheckBox = null!;
    private Label noteLabel = null!;

    private void InitializeComponent()
    {
        fields = FormLayout.CreateTwoColumnLayout();
        plateTextBox = UITheme.CreateTextBox();
        modelTextBox = UITheme.CreateTextBox();
        capacityNumeric = FormLayout.CreateNumeric(100, 1, 100000);
        activeCheckBox = new CheckBox();
        noteLabel = UITheme.CreateLabel(string.Empty, 9F, FontStyle.Regular, UITheme.TextMuted, false);
        ((System.ComponentModel.ISupportInitialize)capacityNumeric).BeginInit();
        fields.SuspendLayout();
        SuspendLayout();
        //
        // fields
        //
        plateTextBox.Text = "А001АА78";
        FormLayout.AddRow(fields, "Госномер *", plateTextBox, 56);
        modelTextBox.Text = "Ford Transit";
        FormLayout.AddRow(fields, "Модель *", modelTextBox, 56);
        FormLayout.AddRow(fields, "Вместимость, мест *", capacityNumeric, 56);
        activeCheckBox.AutoSize = true;
        activeCheckBox.Checked = true;
        activeCheckBox.CheckState = CheckState.Checked;
        activeCheckBox.Name = "activeCheckBox";
        activeCheckBox.Text = "Доступен для назначения";
        activeCheckBox.UseVisualStyleBackColor = true;
        FormLayout.AddRow(fields, "Активность", activeCheckBox, 56);
        //
        // noteLabel
        //
        noteLabel.Dock = DockStyle.Bottom;
        noteLabel.Height = 70;
        noteLabel.Name = "noteLabel";
        noteLabel.Text = "При назначении проверяются вместимость заказа и отсутствие активного маршрута у автомобиля.";
        //
        // VehicleEditForm
        //
        Content.Controls.Add(noteLabel);
        Content.Controls.Add(fields);
        Name = "VehicleEditForm";
        ((System.ComponentModel.ISupportInitialize)capacityNumeric).EndInit();
        fields.ResumeLayout(false);
        fields.PerformLayout();
        ResumeLayout(false);
    }
}
