using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class DirectoryManagementForm
{
    private TabControl _tabs = null!;
    private Button createButton = null!;
    private Button editButton = null!;
    private Button deactivateButton = null!;
    private Button refreshButton = null!;

    private void InitializeComponent()
    {
        _tabs = new TabControl { Dock = DockStyle.Fill, Font = UITheme.BodyFont() };
        createButton = UITheme.CreateButton("Создать", true, 130);
        editButton = UITheme.CreateButton("Редактировать", false, 130);
        deactivateButton = UITheme.CreateButton("Деактивировать", false, 140);
        deactivateButton.ForeColor = UITheme.Danger;
        refreshButton = UITheme.CreateButton("Обновить  F5", false, 125);

        createButton.Click += createButton_Click;
        editButton.Click += editButton_Click;
        deactivateButton.Click += deactivateButton_Click;
        refreshButton.Click += (_, _) => RefreshData();

        var topPanel = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = UITheme.Canvas };
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        buttons.Controls.AddRange(new Control[] { createButton, editButton, deactivateButton, refreshButton });
        topPanel.Controls.Add(buttons);

        CreateTab("Способы оплаты", "payment", ["Название", "Активен"]);
        CreateTab("Типы воды и тары", "water_type", ["Название", "Объём, л", "Активен"]);
        CreateTab("Товары", "product", ["Артикул", "Наименование", "Тип воды / тары", "Цена", "Мест", "Тара", "Активен"]);
        CreateTab("Склады", "warehouse", ["Название", "Адрес", "Координаты", "Активен"]);
        CreateTab("Автомобили", "vehicle", ["Госномер", "Модель", "Вместимость", "Активен"]);

        SuspendLayout();
        Content.Controls.Add(_tabs);
        Content.Controls.Add(topPanel);
        Name = "DirectoryManagementForm";
        ResumeLayout(false);
    }

    private void CreateTab(string title, string tag, string[] columns)
    {
        var tab = new TabPage(title) { Tag = tag, BackColor = UITheme.Canvas };
        var grid = new DataGridView { Dock = DockStyle.Fill };
        UITheme.StyleGrid(grid);
        foreach (var col in columns)
            grid.Columns.Add(col, col);
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) editButton_Click(this, EventArgs.Empty); };
        tab.Controls.Add(grid);
        _tabs.TabPages.Add(tab);
        _grids[tag] = grid;
    }
}
