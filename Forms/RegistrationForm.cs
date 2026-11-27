using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Forms;

public partial class RegistrationForm : BaseEditForm
{
    private readonly AuthService? _auth;

    /// <summary>E-mail только что зарегистрированного клиента — окно входа подставляет его в поле логина.</summary>
    public string? RegisteredEmail { get; private set; }

    // Для визуального дизайнера
    public RegistrationForm() : this(null) { }

    public RegistrationForm(AuthService? auth)
        : base("Регистрация клиента", "Самостоятельное создание учётной записи с ролью «Клиент»", 650, 610)
    {
        _auth = auth;
        InitializeComponent();
    }

    protected override async Task<bool> ValidateAndSaveAsync()
    {
        if (_auth == null)
        {
            UiErrors.Warn("Регистрация недоступна: нет подключения к сервису авторизации.", this);
            return false;
        }

        if (!agreementCheckBox.Checked)
        {
            UiErrors.Warn("Подтвердите корректность введённых данных.", this);
            return false;
        }

        var request = new RegisterRequest(
            fullNameTextBox.Text, emailTextBox.Text, phoneTextBox.Text,
            passwordTextBox.Text, confirmationTextBox.Text);

        var (ok, error) = await _auth.RegisterClientAsync(request);
        if (!ok)
        {
            UiErrors.Warn(error ?? "Не удалось зарегистрироваться", this);
            return false;
        }

        RegisteredEmail = emailTextBox.Text.Trim();
        UiErrors.Info("Регистрация завершена. Теперь можно войти с указанным e-mail и паролем.", this);
        return true;
    }
}
