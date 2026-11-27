using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

/// <summary>Строка списка пользователей (хранится в Tag строки таблицы).</summary>
public sealed record UserRowInfo(int Id, string FullName, string Email, string Phone, UserRole Role, bool Active);

public partial class UserEditForm : BaseEditForm
{
    private readonly IRepository? _repo;
    private readonly ICurrentUser? _currentUser;
    private readonly UserRowInfo? _user;

    // Для визуального дизайнера
    public UserEditForm() : this(null, null, null) { }

    public UserEditForm(IRepository? repo, ICurrentUser? currentUser, UserRowInfo? user) : base(
        user == null ? "Новый пользователь" : "Редактирование пользователя",
        "Учётная запись, роль, контактные данные и активность", 650, 570)
    {
        _repo = repo;
        _currentUser = currentUser;
        _user = user;
        InitializeComponent();

        if (user == null)
        {
            roleComboBox.SelectedIndex = (int)UserRole.Dispatcher;
            passwordTextBox.PlaceholderText = "Не короче 8 символов";
            passwordTextBox.UseSystemPasswordChar = true;
        }
        else
        {
            fullNameTextBox.Text = user.FullName;
            emailTextBox.Text = user.Email;
            phoneTextBox.Text = user.Phone;
            roleComboBox.SelectedIndex = (int)user.Role;
            activeCheckBox.Checked = user.Active;
            passwordTextBox.UseSystemPasswordChar = true;
        }
    }

    protected override async Task<bool> ValidateAndSaveAsync()
    {
        if (_repo == null)
        {
            UiErrors.Warn("Демо-режим: нет подключения к базе данных.", this);
            return false;
        }

        var fullName = fullNameTextBox.Text.Trim();
        var email = emailTextBox.Text.Trim();
        var phone = UiErrors.NormalizePhone(phoneTextBox.Text);
        var password = passwordTextBox.Text;
        var role = (UserRole)roleComboBox.SelectedIndex;
        var roleCode = role.ToString().ToLowerInvariant();   // Admin -> admin, Storekeeper -> storekeeper ...
        var requestorId = _currentUser?.Session?.Id ?? 0;

        if (fullName.Length < 2) { UiErrors.Warn("Введите ФИО.", this); return false; }
        if (!UiErrors.IsValidEmail(email)) { UiErrors.Warn("Введите корректный e-mail.", this); return false; }
        if (!UiErrors.IsValidPhone(phone)) { UiErrors.Warn("Телефон указан в неверном формате.", this); return false; }

        var passwordGiven = !string.IsNullOrEmpty(password);
        if (_user == null && !passwordGiven)
        {
            UiErrors.Warn("Задайте пароль для нового пользователя.", this);
            return false;
        }
        if (passwordGiven && password.Length < UiErrors.MinPasswordLength)
        {
            UiErrors.Warn($"Пароль должен быть не короче {UiErrors.MinPasswordLength} символов.", this);
            return false;
        }

        if (_user == null)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            IReadOnlyDictionary<string, object?> result;
            if (role == UserRole.Client)
            {
                result = await _repo.ExecuteWithOutputsAsync("sp_client_register",
                    new { full_name = fullName, email, phone, password_plain = hash },
                    new[] { "p_user_id" });
            }
            else
            {
                result = await _repo.ExecuteWithOutputsAsync("sp_user_create_employee",
                    new { full_name = fullName, email, phone, role_code = roleCode, password_plain = hash },
                    new[] { "p_user_id" });
            }

            // Новая запись создаётся активной; если администратор снял галочку — деактивируем сразу.
            if (!activeCheckBox.Checked && result.TryGetValue("p_user_id", out var newId) && newId != null)
            {
                await _repo.ExecuteAsync("sp_user_set_active",
                    new { user_id = Convert.ToInt32(newId), active = (sbyte)0, requestor_id = requestorId });
            }
            return true;
        }

        await _repo.ExecuteAsync("sp_user_update", new
        {
            user_id = _user.Id,
            full_name = fullName,
            email,
            phone,
            role_code = roleCode,
            active = (sbyte)(activeCheckBox.Checked ? 1 : 0),
            requestor_id = requestorId
        });

        if (passwordGiven)
        {
            await _repo.ExecuteAsync("sp_user_reset_password",
                new { user_id = _user.Id, new_password_plain = BCrypt.Net.BCrypt.HashPassword(password) });
        }
        return true;
    }
}
