using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

partial class UserEditForm
{
    private TableLayoutPanel fields = null!;
    private TextBox fullNameTextBox = null!;
    private TextBox emailTextBox = null!;
    private TextBox phoneTextBox = null!;
    private ComboBox roleComboBox = null!;
    private TextBox passwordTextBox = null!;
    private CheckBox activeCheckBox = null!;
    private Label noteLabel = null!;

    private void InitializeComponent()
    {
        fields = FormLayout.CreateTwoColumnLayout();
        fullNameTextBox = UITheme.CreateTextBox();
        emailTextBox = UITheme.CreateTextBox();
        phoneTextBox = UITheme.CreateTextBox();
        roleComboBox = FormLayout.CreateComboBox(
            Enum.GetValues<UserRole>().Select(RoleInfo.GetName).ToArray());
        passwordTextBox = UITheme.CreateTextBox("Оставьте пустым, чтобы не менять");
        activeCheckBox = new CheckBox();
        noteLabel = UITheme.CreateLabel(string.Empty, 9F, FontStyle.Regular, UITheme.TextMuted, false);
        fields.SuspendLayout();
        SuspendLayout();
        //
        // fields
        //
        FormLayout.AddRow(fields, "ФИО *", fullNameTextBox, 52);
        FormLayout.AddRow(fields, "E-mail *", emailTextBox, 52);
        FormLayout.AddRow(fields, "Телефон", phoneTextBox, 52);
        FormLayout.AddRow(fields, "Роль *", roleComboBox, 52);
        FormLayout.AddRow(fields, "Новый пароль", passwordTextBox, 52);
        //
        // activeCheckBox
        //
        activeCheckBox.AutoSize = true;
        activeCheckBox.Checked = true;
        activeCheckBox.CheckState = CheckState.Checked;
        activeCheckBox.Name = "activeCheckBox";
        activeCheckBox.Text = "Пользователь активен";
        activeCheckBox.UseVisualStyleBackColor = true;
        FormLayout.AddRow(fields, "Активность", activeCheckBox, 52);
        //
        // noteLabel
        //
        noteLabel.Dock = DockStyle.Bottom;
        noteLabel.Height = 70;
        noteLabel.Name = "noteLabel";
        noteLabel.Text = "Сотрудников создаёт только администратор. Пользователь с историческими данными деактивируется вместо удаления.";
        //
        // UserEditForm
        //
        Content.Controls.Add(noteLabel);
        Content.Controls.Add(fields);
        Name = "UserEditForm";
        fields.ResumeLayout(false);
        fields.PerformLayout();
        ResumeLayout(false);
    }

    private void InitializeComponent(bool readOnly)
    {
        InitializeComponent();
        SaveButton.Visible = !readOnly;
        fullNameTextBox.ReadOnly = readOnly;
        emailTextBox.ReadOnly = readOnly;
        phoneTextBox.ReadOnly = readOnly;
        passwordTextBox.ReadOnly = readOnly;
        roleComboBox.Enabled = !readOnly;
        activeCheckBox.Enabled = !readOnly;
    }
}
