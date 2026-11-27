using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed class ResetPasswordForm : BaseEditForm
{
    private readonly IRepository? _repo;
    private readonly int _userId;
    private readonly TextBox _password;
    private readonly TextBox _confirm;

    public ResetPasswordForm(IRepository? repo, int userId, string userName)
        : base($"Сброс пароля: {userName}", "Установите новый пароль для пользователя", 500, 380)
    {
        _repo = repo;
        _userId = userId;

        var table = FormLayout.CreateTwoColumnLayout();
        _password = UITheme.CreateTextBox("Не короче 8 символов");
        _password.UseSystemPasswordChar = true;
        FormLayout.AddRow(table, "Новый пароль *", _password);
        _confirm = UITheme.CreateTextBox("Повторите пароль");
        _confirm.UseSystemPasswordChar = true;
        FormLayout.AddRow(table, "Подтверждение *", _confirm);
        Content.Controls.Add(table);
    }

    protected override async Task<bool> ValidateAndSaveAsync()
    {
        if (_repo == null)
        {
            UiErrors.Warn("Демо-режим: нет подключения к базе данных.", this);
            return false;
        }
        if (_password.Text.Length < UiErrors.MinPasswordLength)
        {
            UiErrors.Warn($"Пароль должен быть не короче {UiErrors.MinPasswordLength} символов.", this);
            return false;
        }
        if (_password.Text != _confirm.Text)
        {
            UiErrors.Warn("Пароль и подтверждение не совпадают.", this);
            return false;
        }

        await _repo.ExecuteAsync("sp_user_reset_password",
            new { user_id = _userId, new_password_plain = BCrypt.Net.BCrypt.HashPassword(_password.Text) });
        UiErrors.Info("Пароль изменён.", this);
        return true;
    }
}
