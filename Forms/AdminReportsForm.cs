using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

public sealed partial class AdminReportsForm : BaseEditForm
{
    public AdminReportsForm() : base("Административные отчёты", "Полный доступ ко всем отчётам системы", 1200, 800)
    {
        InitializeComponent();
    }

    private void generateButton_Click(object? sender, EventArgs e)
    {
        // TODO: sp_report_* в зависимости от выбранного типа
        MessageBox.Show("Отчёт будет сформирован после интеграции с БД", "WaterRoute");
    }

    private void exportCsvButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("Экспорт CSV будет подключен после реализации отчётов", "WaterRoute");
    }

    private void exportPdfButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("Экспорт PDF будет подключен после реализации отчётов", "WaterRoute");
    }
}