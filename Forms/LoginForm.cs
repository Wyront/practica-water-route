using Microsoft.Extensions.DependencyInjection;
using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Forms;

public partial class LoginForm : Form
{
    private readonly AuthService _authService;
    private readonly IServiceProvider _services;

    public LoginForm(AuthService authService, IServiceProvider services)
    {
        _authService = authService;
        _services = services;
        InitializeComponent();
        Load += LoginForm_Load;
    }

    private async void LoginForm_Load(object? sender, EventArgs e)
    {
        var result = await _authService.TryAutoLoginAsync();
        if (result.Success && result.Session != null)
            OpenMainForm(result.Session);
    }

    private async void loginButton_Click(object? sender, EventArgs e)
    {
        var email = emailTextBox.Text?.Trim();
        var password = passwordTextBox.Text;
        var remember = rememberCheckBox.Checked;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Введите e-mail и пароль", "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        loginButton.Enabled = false;
        loginButton.Text = "Вход...";

        try
        {
            var result = await _authService.LoginAsync(new LoginRequest(email, password, remember));
            if (result.Success && result.Session != null)
                OpenMainForm(result.Session);
            else
                MessageBox.Show(result.ErrorMessage ?? "Ошибка входа", "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            loginButton.Enabled = true;
            loginButton.Text = "Войти";
        }
    }

    private void registerButton_Click(object? sender, EventArgs e)
    {
        using var form = _services.GetRequiredService<RegistrationForm>();
        form.ShowDialog(this);
    }

    private void demoButton_Click(object? sender, EventArgs e)
    {
        var role = (UserRole)roleComboBox.SelectedIndex;
        Hide();
        var demoSession = RoleInfo.CreateDemoSession(role);
        var mainForm = new MainForm(demoSession, _services.GetRequiredService<ICurrentUser>(), _services, _services.GetRequiredService<INotificationService>());
        mainForm.ShowDialog(this);
        Show();
        Activate();
    }

    private void OpenMainForm(UserSession session)
    {
        Hide();
        var mainForm = new MainForm(session, _services.GetRequiredService<ICurrentUser>(), _services, _services.GetRequiredService<INotificationService>());
        var result = mainForm.ShowDialog(this);
        if (result == DialogResult.OK)
        {
            Show();
            Activate();
        }
        else
        {
            Application.Exit();
        }
    }
}