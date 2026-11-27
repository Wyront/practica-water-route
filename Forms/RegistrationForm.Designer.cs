#if NET10_0_OR_GREATER
#nullable enable
#endif

using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class RegistrationForm
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    private TableLayoutPanel fields = null!;
    private TextBox fullNameTextBox = null!;
    private TextBox emailTextBox = null!;
    private TextBox phoneTextBox = null!;
    private TextBox passwordTextBox = null!;
    private TextBox confirmationTextBox = null!;
    private CheckBox agreementCheckBox = null!;
    private Label noteLabel = null!;

    private void InitializeComponent()
    {
        fields = new TableLayoutPanel();
        fullNameTextBox = new TextBox();
        emailTextBox = new TextBox();
        phoneTextBox = new TextBox();
        passwordTextBox = new TextBox();
        confirmationTextBox = new TextBox();
        agreementCheckBox = new CheckBox();
        noteLabel = new Label();
        fields.SuspendLayout();
        SuspendLayout();
        SetFormHeader("Регистрация клиента", "Самостоятельное создание учётной записи с ролью «Клиент»");
        ClientSize = new Size(650, 610);
        fields.Dock = DockStyle.Top;
        fields.Height = 280;
        fields.ColumnCount = 2;
        fields.RowCount = 5;
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190F));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        for (var i = 0; i < 5; i++) fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
        AddCaption(0, "ФИО *");
        fullNameTextBox.Dock = DockStyle.Fill;
        fullNameTextBox.PlaceholderText = "Иванов Иван Иванович";
        fields.Controls.Add(fullNameTextBox, 1, 0);
        AddCaption(1, "E-mail *");
        emailTextBox.Dock = DockStyle.Fill;
        emailTextBox.PlaceholderText = "client@example.ru";
        fields.Controls.Add(emailTextBox, 1, 1);
        AddCaption(2, "Телефон");
        phoneTextBox.Dock = DockStyle.Fill;
        phoneTextBox.PlaceholderText = "+7 900 000-00-00";
        fields.Controls.Add(phoneTextBox, 1, 2);
        AddCaption(3, "Пароль *");
        passwordTextBox.Dock = DockStyle.Fill;
        passwordTextBox.UseSystemPasswordChar = true;
        fields.Controls.Add(passwordTextBox, 1, 3);
        AddCaption(4, "Подтверждение *");
        confirmationTextBox.Dock = DockStyle.Fill;
        confirmationTextBox.UseSystemPasswordChar = true;
        fields.Controls.Add(confirmationTextBox, 1, 4);
        agreementCheckBox.AutoSize = true;
        agreementCheckBox.Dock = DockStyle.Top;
        agreementCheckBox.Height = 42;
        agreementCheckBox.Font = UITheme.BodyFont();
        agreementCheckBox.Text = "Подтверждаю корректность введённых данных";
        noteLabel.Dock = DockStyle.Top;
        noteLabel.Height = 84;
        noteLabel.Font = UITheme.BodyFont(9F);
        noteLabel.ForeColor = UITheme.TextMuted;
        noteLabel.Padding = new Padding(0, 10, 0, 0);
        noteLabel.Text = "После регистрации клиент сможет добавить адреса доставки и оформить заказ. Сотрудники создаются только администратором.";
        contentPanel.Controls.Add(noteLabel);
        contentPanel.Controls.Add(agreementCheckBox);
        contentPanel.Controls.Add(fields);
        fields.ResumeLayout(false);
        fields.PerformLayout();
        ResumeLayout(false);
    }

    private void AddCaption(int row, string text)
    {
        var label = new Label
        {
            Text = text,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Font = UITheme.BodyFont(9.5F),
            ForeColor = UITheme.TextMuted,
            Margin = new Padding(0, 0, 12, 8)
        };
        fields.Controls.Add(label, 0, row);
    }
}
