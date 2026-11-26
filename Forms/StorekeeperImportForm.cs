using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed class StorekeeperImportForm : BaseEditForm
{
    public StorekeeperImportForm() : base("Импорт CSV", "Товары и складские поступления. При любой ошибке весь файл отклоняется", 900, 650)
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        var settings = new Panel { Dock = DockStyle.Top, Height = 142, BackColor = UITheme.Surface, Padding = new Padding(14) };
        var typeLabel = UITheme.CreateLabel("Тип импорта", 9F, FontStyle.Regular, UITheme.TextMuted);
        typeLabel.Location = new Point(14, 8);
        settings.Controls.Add(typeLabel);
        var type = FormLayout.CreateComboBox("Складские поступления", "Товары");
        type.SetBounds(14, 30, 280, 34);
        settings.Controls.Add(type);
        var template = UITheme.CreateButton("Скачать шаблон", false, 160);
        template.Location = new Point(310, 28);
        settings.Controls.Add(template);
        var pathLabel = UITheme.CreateLabel("CSV-файл", 9F, FontStyle.Regular, UITheme.TextMuted);
        pathLabel.Location = new Point(14, 76);
        settings.Controls.Add(pathLabel);
        var path = UITheme.CreateTextBox("Файл не выбран");
        path.SetBounds(14, 98, 620, 34);
        settings.Controls.Add(path);
        var browse = UITheme.CreateButton("Обзор…", false, 110);
        browse.Location = new Point(650, 94);
        settings.Controls.Add(browse);
        var load = UITheme.CreateButton("Проверить файл", true, 160);
        load.Location = new Point(778, 94);
        settings.Controls.Add(load);

        var actions = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 58, FlowDirection = FlowDirection.LeftToRight };
        var import = UITheme.CreateButton("Импортировать одной транзакцией", true, 265);
        actions.Controls.Add(import);
        var errors = UITheme.CreateButton("Отчёт об ошибках", false, 170);
        actions.Controls.Add(errors);
        var status = UITheme.CreateLabel("Выберите CSV-файл для предварительной проверки", 9F, FontStyle.Regular, UITheme.TextMuted);
        status.Margin = new Padding(12, 8, 0, 0);
        actions.Controls.Add(status);

        var preview = new DataGridView();
        UITheme.StyleGrid(preview);
        preview.Columns.Add("Line", "Строка");
        preview.Columns.Add("Warehouse", "Склад");
        preview.Columns.Add("Sku", "Артикул");
        preview.Columns.Add("Quantity", "Количество");
        preview.Columns.Add("Result", "Результат проверки");
        Content.Controls.Add(preview);
        Content.Controls.Add(actions);
        Content.Controls.Add(settings);
        Content.Controls.SetChildIndex(settings, 0);
        Content.Controls.SetChildIndex(actions, 1);
        Content.Controls.SetChildIndex(preview, 2);
    }
}