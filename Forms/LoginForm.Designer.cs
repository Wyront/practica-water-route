#if NET10_0_OR_GREATER
#nullable enable
#endif

using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

partial class LoginForm
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    private Panel brandPanel = null!;
    private Label brandTitleLabel = null!;
    private Label brandSubtitleLabel = null!;
    private Label brandDescriptionLabel = null!;
    private Label versionLabel = null!;
    private Panel loginPanel = null!;
    private Label titleLabel = null!;
    private Label subtitleLabel = null!;
    private Label emailLabel = null!;
    private TextBox emailTextBox = null!;
    private Label passwordLabel = null!;
    private TextBox passwordTextBox = null!;
    private CheckBox rememberCheckBox = null!;
    private Button loginButton = null!;
    private Button registerButton = null!;
    private Panel dividerPanel = null!;
    private Label demoTitleLabel = null!;
    private Label demoHintLabel = null!;
    private ComboBox roleComboBox = null!;
    private Button demoButton = null!;

    private void InitializeComponent()
    {
        brandPanel = new Panel(); brandTitleLabel = new Label(); brandSubtitleLabel = new Label(); brandDescriptionLabel = new Label(); versionLabel = new Label();
        loginPanel = new Panel(); titleLabel = new Label(); subtitleLabel = new Label(); emailLabel = new Label(); emailTextBox = new TextBox();
        passwordLabel = new Label(); passwordTextBox = new TextBox(); rememberCheckBox = new CheckBox(); loginButton = new Button(); registerButton = new Button();
        dividerPanel = new Panel(); demoTitleLabel = new Label(); demoHintLabel = new Label(); roleComboBox = new ComboBox(); demoButton = new Button();
        brandPanel.SuspendLayout(); loginPanel.SuspendLayout(); SuspendLayout();
        brandPanel.BackColor = UITheme.Navigation; brandPanel.Dock = DockStyle.Left; brandPanel.Width = 430;
        brandPanel.Controls.Add(brandTitleLabel); brandPanel.Controls.Add(brandSubtitleLabel); brandPanel.Controls.Add(brandDescriptionLabel); brandPanel.Controls.Add(versionLabel);
        brandTitleLabel.AutoSize = true; brandTitleLabel.Font = UITheme.TitleFont(32F); brandTitleLabel.ForeColor = Color.White; brandTitleLabel.Location = new Point(54, 190); brandTitleLabel.Text = "WaterRoute";
        brandSubtitleLabel.AutoSize = true; brandSubtitleLabel.Font = UITheme.BodyFont(16F); brandSubtitleLabel.ForeColor = Color.FromArgb(191, 219, 254); brandSubtitleLabel.Location = new Point(58, 258); brandSubtitleLabel.Text = "Доставка питьевой воды";
        brandDescriptionLabel.Font = UITheme.BodyFont(11F); brandDescriptionLabel.ForeColor = Color.FromArgb(203, 213, 225); brandDescriptionLabel.SetBounds(58, 330, 320, 140); brandDescriptionLabel.Text = "Заказы, склады, маршруты и доставка\nв одной информационной системе.\n\nСанкт-Петербург";
        versionLabel.AutoSize = true; versionLabel.Anchor = AnchorStyles.Left | AnchorStyles.Bottom; versionLabel.Font = UITheme.BodyFont(9F); versionLabel.ForeColor = Color.FromArgb(148, 163, 184); versionLabel.Location = new Point(58, 590); versionLabel.Text = "Учебный проект · .NET 10 · WinForms";
        loginPanel.BackColor = UITheme.Surface; loginPanel.Dock = DockStyle.Fill;
        loginPanel.Controls.Add(titleLabel); loginPanel.Controls.Add(subtitleLabel); loginPanel.Controls.Add(emailLabel); loginPanel.Controls.Add(emailTextBox);
        loginPanel.Controls.Add(passwordLabel); loginPanel.Controls.Add(passwordTextBox); loginPanel.Controls.Add(rememberCheckBox); loginPanel.Controls.Add(loginButton);
        loginPanel.Controls.Add(registerButton); loginPanel.Controls.Add(dividerPanel); loginPanel.Controls.Add(demoTitleLabel); loginPanel.Controls.Add(demoHintLabel);
        loginPanel.Controls.Add(roleComboBox); loginPanel.Controls.Add(demoButton);
        titleLabel.AutoSize = true; titleLabel.Font = UITheme.TitleFont(24F); titleLabel.Location = new Point(65, 65); titleLabel.Text = "Вход в систему";
        subtitleLabel.AutoSize = true; subtitleLabel.Font = UITheme.BodyFont(10F); subtitleLabel.ForeColor = UITheme.TextMuted; subtitleLabel.Location = new Point(67, 116); subtitleLabel.Text = "Введите данные учётной записи";
        emailLabel.AutoSize = true; emailLabel.Location = new Point(67, 175); emailLabel.Text = "E-mail"; emailLabel.ForeColor = UITheme.TextMuted;
        emailTextBox.SetBounds(67, 205, 475, 38); emailTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; emailTextBox.PlaceholderText = "name@example.ru";
        passwordLabel.AutoSize = true; passwordLabel.Location = new Point(67, 260); passwordLabel.Text = "Пароль"; passwordLabel.ForeColor = UITheme.TextMuted;
        passwordTextBox.SetBounds(67, 290, 475, 38); passwordTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; passwordTextBox.UseSystemPasswordChar = true; passwordTextBox.PlaceholderText = "Введите пароль";
        rememberCheckBox.AutoSize = true; rememberCheckBox.Checked = true; rememberCheckBox.Location = new Point(67, 345); rememberCheckBox.Text = "Запомнить меня и входить автоматически";
        ConfigureButton(loginButton, "Войти", true); loginButton.SetBounds(67, 390, 180, 42); loginButton.Click += loginButton_Click;
        ConfigureButton(registerButton, "Регистрация клиента", false); registerButton.SetBounds(265, 390, 190, 42); registerButton.Click += registerButton_Click;
        dividerPanel.BackColor = UITheme.Border; dividerPanel.SetBounds(67, 465, 475, 1); dividerPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        demoTitleLabel.AutoSize = true; demoTitleLabel.Font = UITheme.BodyFont(11F, FontStyle.Bold); demoTitleLabel.Location = new Point(67, 487); demoTitleLabel.Text = "Просмотр UI-каркаса";
        demoHintLabel.AutoSize = true; demoHintLabel.Font = UITheme.BodyFont(8.5F); demoHintLabel.ForeColor = UITheme.TextMuted; demoHintLabel.Location = new Point(67, 520); demoHintLabel.Text = "Откройте приложение от имени выбранной роли без подключения к БД.";
        roleComboBox.DropDownStyle = ComboBoxStyle.DropDownList; roleComboBox.SetBounds(67, 558, 250, 36); roleComboBox.Font = UITheme.BodyFont();
        roleComboBox.Items.AddRange(Enum.GetValues<UserRole>().Select(RoleInfo.GetName).Cast<object>().ToArray()); roleComboBox.SelectedIndex = 1;
        ConfigureButton(demoButton, "Открыть макет", true); demoButton.SetBounds(335, 552, 195, 42); demoButton.Click += demoButton_Click;
        AcceptButton = loginButton; AutoScaleMode = AutoScaleMode.Font; BackColor = UITheme.Surface; ClientSize = new Size(1100, 700);
        Controls.Add(loginPanel); Controls.Add(brandPanel); Font = UITheme.BodyFont(); MinimumSize = new Size(980, 640);
        Name = "LoginForm"; StartPosition = FormStartPosition.CenterScreen; Text = "WaterRoute — вход в систему";
        brandPanel.ResumeLayout(false); brandPanel.PerformLayout(); loginPanel.ResumeLayout(false); loginPanel.PerformLayout(); ResumeLayout(false);
    }

    private static void ConfigureButton(Button button, string text, bool primary)
    {
        button.Text = text; button.BackColor = primary ? UITheme.Primary : UITheme.Surface; button.ForeColor = primary ? Color.White : UITheme.Text;
        button.FlatStyle = FlatStyle.Flat; button.Font = UITheme.BodyFont(9.5F, primary ? FontStyle.Bold : FontStyle.Regular);
    }
}
