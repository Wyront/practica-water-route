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
        Shown += LoginForm_Shown;
    }

    private async void LoginForm_Shown(object? sender, EventArgs e)
    {
        var result = await _authService.TryAutoLoginAsync();
        if (!IsDisposed && result.Success && result.Session != null)
            OpenMainForm(result.Session);
    }

    private async void loginButton_Click(object? sender, EventArgs e)
    {
        var email = emailTextBox.Text?.Trim();
        var password = passwordTextBox.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Введите e-mail и пароль", "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        loginButton.Enabled = false;
        loginButton.Text = "Вход...";

        try
        {
            var result = await _authService.LoginAsync(new LoginRequest(email, password, false));
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
        if (form.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(form.RegisteredEmail))
        {
            emailTextBox.Text = form.RegisteredEmail;
            passwordTextBox.Clear();
            passwordTextBox.Focus();
        }
    }

    private void demoButton_Click(object? sender, EventArgs e)
    {
        var index = roleComboBox.SelectedIndex;
        var role = Enum.IsDefined(typeof(UserRole), index) ? (UserRole)index : UserRole.Admin;

        Hide();
        try
        {
            using var mainForm = CreateMainForm(RoleInfo.CreateDemoSession(role));
            mainForm.ShowDialog(this);
        }
        finally
        {
            // Что бы ни случилось в главном окне, окно входа должно вернуться, иначе процесс останется невидимым.
            Show();
            Activate();
        }
    }

    private void OpenMainForm(UserSession session)
    {
        Hide();
        DialogResult result;
        try
        {
            using var mainForm = CreateMainForm(session);
            result = mainForm.ShowDialog(this);
        }
        catch
        {
            Show();
            throw;
        }

        if (result == DialogResult.OK)
        {
            passwordTextBox.Clear();
            Show();
            Activate();
        }
        else
        {
            Application.Exit();
        }
    }

    private MainForm CreateMainForm(UserSession session) => new(
        session,
        _services.GetRequiredService<ICurrentUser>(),
        _services,
        _services.GetRequiredService<INotificationService>());

    private void loginPanel_Paint(object sender, PaintEventArgs e)
    {

    }
}
