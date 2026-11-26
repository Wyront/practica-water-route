namespace WaterRoute.Controls;

public partial class ImportPage : BasePage
{
    public ImportPage() : base(
        "Импорт CSV",
        "Товары и складские поступления. При любой ошибке весь файл отклоняется без изменений в БД")
    {
        InitializeComponent();
    }

    private void browseButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog { Filter = "CSV-файлы (*.csv)|*.csv|Все файлы (*.*)|*.*", Title = "Выберите CSV-файл" };
        if (dialog.ShowDialog(this) == DialogResult.OK) pathTextBox.Text = dialog.FileName;
    }

    private void previewButton_Click(object? sender, EventArgs e)
    {
        previewGrid.Rows.Clear();
        previewGrid.Rows.Add("2", "Центральный склад", "WT-019", "20", "Готово");
        previewGrid.Rows.Add("3", "Центральный склад", "WT-005", "30", "Готово");
        previewGrid.Rows.Add("4", "Северный склад", "UNKNOWN", "10", "Ошибка: товар не найден");
        previewGrid.Rows[2].DefaultCellStyle.ForeColor = Infrastructure.UITheme.Danger;
        statusLabel.Text = "Демонстрация проверки: найдена 1 ошибка. Импорт заблокирован.";
    }
}
