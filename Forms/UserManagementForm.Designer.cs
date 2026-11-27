using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

partial class UserManagementForm
{
    private DataGridView usersGrid = null!;
    private Button createButton = null!;
    private Button editButton = null!;
    private Button deactivateButton = null!;
    private Button resetPasswordButton = null!;
    private Button refreshButton = null!;

    private void InitializeComponent()
    {
        usersGrid = new DataGridView();
        createButton = UITheme.CreateButton("Создать сотрудника", true, 180);
        editButton = UITheme.CreateButton("Редактировать", false, 140);
        deactivateButton = UITheme.CreateButton("Деактивировать", false, 140);
        resetPasswordButton = UITheme.CreateButton("Сбросить пароль", false, 150);
        refreshButton = UITheme.CreateButton("Обновить  F5", false, 125);

        ((System.ComponentModel.ISupportInitialize)usersGrid).BeginInit();
        SuspendLayout();

        // actions
        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = UITheme.Canvas
        };
        createButton.Click += createButton_Click;
        editButton.Click += editButton_Click;
        deactivateButton.Click += deactivateButton_Click;
        deactivateButton.ForeColor = UITheme.Danger;
        resetPasswordButton.Click += resetPasswordButton_Click;
        refreshButton.Click += (_, _) => RefreshData();
        actions.Controls.AddRange(new Control[] { createButton, editButton, deactivateButton, resetPasswordButton, refreshButton });

        // grid
        UITheme.StyleGrid(usersGrid);
        usersGrid.Columns.Add("Id", "ID");
        usersGrid.Columns.Add("FullName", "ФИО");
        usersGrid.Columns.Add("Email", "E-mail");
        usersGrid.Columns.Add("Phone", "Телефон");
        usersGrid.Columns.Add("Role", "Роль");
        usersGrid.Columns.Add("Active", "Активен");
        usersGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        usersGrid.MultiSelect = false;
        usersGrid.CellDoubleClick += (_, _) => editButton_Click(this, EventArgs.Empty);

        // demo data
        usersGrid.Rows.Add("1", "Администратор", "admin@water.test", "+79000000001", "Администратор", "Да");
        usersGrid.Rows[0].Tag = new UserRowInfo(1, "Администратор", "admin@water.test", "+79000000001", UserRole.Admin, true);
        usersGrid.Rows.Add("2", "Диспетчер Анна", "disp@water.test", "+79000000002", "Диспетчер", "Да");
        usersGrid.Rows[1].Tag = new UserRowInfo(2, "Диспетчер Анна", "disp@water.test", "+79000000002", UserRole.Dispatcher, true);
        usersGrid.Rows.Add("3", "Кладовщик Дмитрий", "wh@water.test", "+79000000003", "Кладовщик", "Да");
        usersGrid.Rows[2].Tag = new UserRowInfo(3, "Кладовщик Дмитрий", "wh@water.test", "+79000000003", UserRole.Storekeeper, true);
        usersGrid.Rows.Add("4", "Курьер Алексей", "courier1@water.test", "+79000000004", "Курьер", "Да");
        usersGrid.Rows[3].Tag = new UserRowInfo(4, "Курьер Алексей", "courier1@water.test", "+79000000004", UserRole.Courier, true);
        usersGrid.Rows.Add("5", "Курьер Сергей", "courier2@water.test", "+79000000005", "Курьер", "Да");
        usersGrid.Rows[4].Tag = new UserRowInfo(5, "Курьер Сергей", "courier2@water.test", "+79000000005", UserRole.Courier, true);

        Content.Controls.Add(usersGrid);
        Content.Controls.Add(actions);
        Name = "UserManagementForm";
        ((System.ComponentModel.ISupportInitialize)usersGrid).EndInit();
        ResumeLayout(false);
    }
}