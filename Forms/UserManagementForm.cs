using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

public sealed partial class UserManagementForm : BaseEditForm
{
    private readonly IRepository? _repo;
    private readonly ICurrentUser? _currentUser;

    public UserManagementForm() : this(null, null) { }

    // DI подставит IRepository и ICurrentUser (форма зарегистрирована в AddWaterRouteServices)
    public UserManagementForm(IRepository? repo, ICurrentUser? currentUser)
        : base("Управление пользователями", "Создание, редактирование, деактивация и сброс паролей", 900, 700)
    {
        _repo = repo;
        _currentUser = currentUser;
        InitializeComponent();

        // Кнопка меняет подпись в зависимости от того, активен ли выбранный пользователь.
        usersGrid.SelectionChanged += (_, _) => UpdateToggleButton();
        Load += (_, _) => RefreshData();
    }

    private UserRowInfo? SelectedUser =>
        usersGrid.SelectedRows.Count > 0 ? usersGrid.SelectedRows[0].Tag as UserRowInfo : null;

    private void UpdateToggleButton()
    {
        deactivateButton.Text = SelectedUser is { Active: false } ? "Активировать" : "Деактивировать";
    }

    private async void RefreshData()
    {
        if (_repo == null) return; // демо-режим — остаются данные из Designer
        var selectedId = SelectedUser?.Id;
        try
        {
            var (rows, _) = await _repo.QueryPagedAsync<dynamic>("sp_user_list",
                new { page = 1, limit = 500, search = "", role_code = "", active = (sbyte)-1 });

            usersGrid.Rows.Clear();
            foreach (IDictionary<string, object?> r in rows)
            {
                var roleOk = Enum.TryParse<UserRole>(Convert.ToString(r["role_code"]), true, out var role);
                var info = new UserRowInfo(
                    Convert.ToInt32(r["id"]),
                    Convert.ToString(r["full_name"]) ?? "",
                    Convert.ToString(r["email"]) ?? "",
                    Convert.ToString(r["phone"]) ?? "",
                    roleOk ? role : UserRole.Client,
                    Convert.ToBoolean(r["active"]));

                var index = usersGrid.Rows.Add(
                    info.Id.ToString(), info.FullName, info.Email, info.Phone,
                    roleOk ? RoleInfo.GetName(role) : Convert.ToString(r["role_code"]),
                    info.Active ? "Да" : "Нет");
                usersGrid.Rows[index].Tag = info;
                if (!info.Active)
                    usersGrid.Rows[index].DefaultCellStyle.ForeColor = UITheme.TextMuted;
            }

            // Возвращаем выделение на прежнего пользователя
            if (selectedId != null)
                foreach (DataGridViewRow row in usersGrid.Rows)
                    if ((row.Tag as UserRowInfo)?.Id == selectedId) { row.Selected = true; break; }
            UpdateToggleButton();
        }
        catch (Exception ex)
        {
            UiErrors.Show(ex, this);
        }
    }

    private async void deactivateButton_Click(object? sender, EventArgs e)
    {
        var user = SelectedUser;
        if (user == null) { UiErrors.Info("Выберите пользователя.", this); return; }
        if (_repo == null) { UiErrors.Info("Демо-режим: нет подключения к БД.", this); return; }

        var activate = !user.Active;
        var question = activate ? $"Активировать пользователя {user.FullName}?" : $"Деактивировать пользователя {user.FullName}?";
        if (MessageBox.Show(this, question, "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            await _repo.ExecuteAsync("sp_user_set_active",
                new { user_id = user.Id, active = (sbyte)(activate ? 1 : 0), requestor_id = _currentUser?.Session?.Id ?? 0 });
            RefreshData();
        }
        catch (Exception ex)
        {
            // Текст из SIGNAL SQLSTATE '45000' (например «Нельзя деактивировать самого себя»)
            UiErrors.Show(ex, this);
        }
    }

    private void createButton_Click(object? sender, EventArgs e)
    {
        using var form = new UserEditForm(_repo, _currentUser, null);
        if (form.ShowDialog(this) == DialogResult.OK) RefreshData();
    }

    private void editButton_Click(object? sender, EventArgs e)
    {
        var user = SelectedUser;
        if (user == null) { UiErrors.Info("Выберите пользователя.", this); return; }
        using var form = new UserEditForm(_repo, _currentUser, user);
        if (form.ShowDialog(this) == DialogResult.OK) RefreshData();
    }

    private void resetPasswordButton_Click(object? sender, EventArgs e)
    {
        var user = SelectedUser;
        if (user == null) { UiErrors.Info("Выберите пользователя.", this); return; }
        using var form = new ResetPasswordForm(_repo, user.Id, user.FullName);
        form.ShowDialog(this);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.F5) { RefreshData(); return true; }
        return base.ProcessCmdKey(ref msg, keyData);
    }
}
