using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Controls;

public partial class ImportPage : BasePage
{
    private readonly CsvImportService _importService = new();
    private CsvImportResult? _result;

    public ImportPage() : base(
        "Импорт CSV",
        "Товары и складские поступления. При любой ошибке весь файл отклоняется без изменений в БД")
    {
        InitializeComponent();
        ResetForm();
    }

    private CsvImportType CurrentType => typeComboBox.SelectedIndex == 0
        ? CsvImportType.StockReceipts
        : CsvImportType.Products;

    private void browseButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog { Filter = "CSV-файлы (*.csv)|*.csv|Все файлы (*.*)|*.*", Title = "Выберите CSV-файл" };
        if (dialog.ShowDialog(this) == DialogResult.OK)
            pathTextBox.Text = dialog.FileName;
    }

    private async void previewButton_Click(object? sender, EventArgs e)
    {
        if (!File.Exists(pathTextBox.Text))
        {
            MessageBox.Show("Выберите существующий CSV-файл.", "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        previewButton.Enabled = false;
        try
        {
            _result = await _importService.ReadAndValidateAsync(pathTextBox.Text, CurrentType);
            ShowPreview(_result);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось прочитать CSV: {ex.Message}", "Ошибка импорта",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            previewButton.Enabled = true;
        }
    }

    private async void templateButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "CSV-файлы (*.csv)|*.csv",
            FileName = CurrentType == CsvImportType.Products ? "products-template.csv" : "stock-receipts-template.csv",
            Title = "Сохранить CSV-шаблон"
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            var path = await _importService.SaveTemplateAsync(CurrentType, dialog.FileName);
            MessageBox.Show($"Шаблон сохранён:\n{path}", "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось сохранить шаблон: {ex.Message}", "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void importButton_Click(object? sender, EventArgs e)
    {
        if (_result?.IsValid != true)
        {
            MessageBox.Show("Сначала проверьте файл. Импорт разрешён только при отсутствии ошибок.", "WaterRoute",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        MessageBox.Show(
            $"Файл проверен: {_result.ValidRows.Count} строк.\n\nПосле подключения MySQL данные будут импортированы одной транзакцией через соответствующую хранимую процедуру. На текущем этапе база не изменяется.",
            "Валидация пройдена", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private async void errorsButton_Click(object? sender, EventArgs e)
    {
        if (_result == null || _result.Errors.Count == 0)
        {
            MessageBox.Show("Ошибок для выгрузки нет.", "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Filter = "CSV-файлы (*.csv)|*.csv",
            FileName = "import-errors.csv",
            Title = "Сохранить отчёт об ошибках"
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            await _importService.WriteErrorsAsync(_result, dialog.FileName);
            MessageBox.Show($"Отчёт сохранён:\n{dialog.FileName}", "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось сохранить отчёт: {ex.Message}", "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void typeComboBox_SelectedIndexChanged(object? sender, EventArgs e) => ResetForm();

    private void ShowPreview(CsvImportResult result)
    {
        previewGrid.Rows.Clear();
        foreach (var row in result.PreviewRows)
        {
            var index = previewGrid.Rows.Add(row.LineNumber, row.Warehouse, row.Sku, row.Quantity, row.Result);
            if (row.IsError)
                previewGrid.Rows[index].DefaultCellStyle.ForeColor = Infrastructure.UITheme.Danger;
            else
                previewGrid.Rows[index].DefaultCellStyle.ForeColor = Infrastructure.UITheme.Success;
        }
        statusLabel.Text = result.StatusText;
        importButton.Enabled = result.IsValid;
        errorsButton.Enabled = result.Errors.Count > 0;
    }

    private void ResetForm()
    {
        _result = null;
        previewGrid.Rows.Clear();
        statusLabel.Text = "Выберите CSV-файл для проверки";
        importButton.Enabled = false;
        errorsButton.Enabled = false;
        quantityColumn.HeaderText = CurrentType == CsvImportType.Products ? "Места" : "Количество";
    }
}
