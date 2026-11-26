using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

public sealed partial class UserManagementForm : BaseEditForm
{
    public UserManagementForm() : base("Управление пользователями", "Создание, редактирование, деактивация и сброс паролей", 900, 700)
    {
        InitializeComponent();
    }

    private void createButton_Click(object? sender, EventArgs e)
    {
        using var form = new UserEditForm();
        if (form.ShowDialog(this) == DialogResult.OK)
            RefreshData();
    }

    private void editButton_Click(object? sender, EventArgs e)
    {
        if (usersGrid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Выберите пользователя", "WaterRoute");
            return;
        }
        var row = (string[])usersGrid.SelectedRows[0].Tag!;
        using var form = new UserEditForm(row);
        if (form.ShowDialog(this) == DialogResult.OK)
            RefreshData();
    }

    private void deactivateButton_Click(object? sender, EventArgs e)
    {
        if (usersGrid.SelectedRows.Count == 0) return;
        var row = (string[])usersGrid.SelectedRows[0].Tag!;
        var userId = int.Parse(row[0]);
        if (MessageBox.Show($"Деактивировать пользователя {row[1]}?", "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            // TODO: sp_user_set_active
            MessageBox.Show("Функция будет подключена после интеграции с БД", "WaterRoute");
        }
    }

    private void resetPasswordButton_Click(object? sender, EventArgs e)
    {
        if (usersGrid.SelectedRows.Count == 0) return;
        var row = (string[])usersGrid.SelectedRows[0].Tag!;
        using var form = new ResetPasswordForm(row[1]);
        form.ShowDialog(this);
    }

    private void RefreshData()
    {
        // TODO: загрузка через sp_user_list
    }
}