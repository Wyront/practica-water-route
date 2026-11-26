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

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = UITheme.Canvas
        };
        actions.Controls.AddRange(new Control[] { createButton, editButton, deactivateButton, refreshButton });

        CreateTab("Способы оплаты", "payment", ["Код", "Название", "Активен"],
            [["cash", "Наличные", "Да"], ["card", "Карта", "Да"], ["online", "Онлайн", "Да"], ["transfer", "Безнал", "Да"]]);
        CreateTab("Типы воды и тары", "water_type", ["Название", "Объём, л", "Активен"],
            [["Бутыль 19 л", "19", "Да"], ["ПЭТ 5 л", "5", "Да"], ["Без тары", "0", "Да"]]);
        CreateTab("Товары", "product", ["Артикул", "Наименование", "Цена", "Мест", "Тара", "Активен"],
            [["WT-019", "Вода питьевая 19 л", "250,00 ₽", "2", "Возвратная", "Да"],
             ["WT-005", "Вода питьевая 5 л", "90,00 ₽", "1", "Нет", "Да"],
             ["AC-001", "Помпа механическая", "450,00 ₽", "1", "Нет", "Да"]]);
        CreateTab("Склады", "warehouse", ["Название", "Адрес", "Координаты", "Активен"],
            [["Центральный склад", "Складская улица, 1", "59.9343, 30.3351", "Да"],
             ["Северный склад", "Северная улица, 25", "60.0152, 30.3204", "Да"],
             ["Приморский склад", "Приморский проспект, 40", "59.9851, 30.2108", "Да"]]);
        CreateTab("Автомобили", "vehicle", ["Госномер", "Модель", "Вместимость", "Активен"],
            [["А001АА78", "Ford Transit", "100", "Да"],
             ["А002АА78", "Ford Transit", "120", "Да"],
             ["В003ВВ78", "ГАЗель Next", "90", "Да"]]);

        var topPanel = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = UITheme.Canvas };
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        var createBtn = UITheme.CreateButton("Создать", true, 130);
        var editBtn = UITheme.CreateButton("Редактировать", false, 130);
        var deactBtn = UITheme.CreateButton("Деактивировать", false, 140);
        deactBtn.ForeColor = UITheme.Danger;
        var refBtn = UITheme.CreateButton("Обновить  F5", false, 125);
        createBtn.Click += createButton_Click;
        editBtn.Click += editButton_Click;
        deactBtn.Click += deactivateButton_Click;
        buttons.Controls.AddRange(new Control[] { createBtn, editBtn, deactBtn, refreshButton });
        topPanel.Controls.Add(buttons);

        Content.Controls.Add(_tabs);
        Content.Controls.Add(topPanel);
        Name = "DirectoryManagementForm";
        ResumeLayout(false);
    }

    private void CreateTab(string title, string tag, string[] columns, string[][] rows)
    {
        var tab = new TabPage(title) { Tag = tag, BackColor = UITheme.Canvas };
        var grid = new DataGridView { Dock = DockStyle.Fill };
        UITheme.StyleGrid(grid);
        foreach (var col in columns)
            grid.Columns.Add(col.ToLower(), col);
        foreach (var row in rows)
        {
            grid.Rows.Add(row);
        }
        foreach (DataGridViewRow r in grid.Rows)
        {
            var capturedRow = (string[])r.Tag!;
            // Tag already set correctly above
        }
        tab.Controls.Add(grid);
        _tabs.TabPages.Add(tab);
    }
}