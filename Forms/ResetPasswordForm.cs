using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed class ResetPasswordForm : BaseEditForm
{
    public ResetPasswordForm(string userName) : base($"Сброс пароля: {userName}", "Установите новый пароль для пользователя", 500, 380)
    {
        var table = FormLayout.CreateTwoColumnLayout();
        var password = UITheme.CreateTextBox("Новый пароль");
        password.UseSystemPasswordChar = true;
        FormLayout.AddRow(table, "Новый пароль *", password);
        var confirm = UITheme.CreateTextBox("Подтверждение *");
        confirm.UseSystemPasswordChar = true;
        FormLayout.AddRow(table, "Подтверждение *", confirm);
        Content.Controls.Add(table);

        SaveButton.Click += (_, _) =>
        {
            if (password.Text != confirm.Text)
            {
                MessageBox.Show("Пароли не совпадают", "WaterRoute");
                return;
            }
            if (password.Text.Length < 8)
            {
                MessageBox.Show("Пароль должен быть не менее 8 символов", "WaterRoute");
                return;
            }
            // TODO: sp_user_reset_password
            MessageBox.Show("Пароль будет сброшен после интеграции с БД", "WaterRoute");
            DialogResult = DialogResult.OK;
            Close();
        };
    }
}