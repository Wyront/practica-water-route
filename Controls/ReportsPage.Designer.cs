using WaterRoute.Forms;
using WaterRoute.Infrastructure;

namespace WaterRoute.Controls;

partial class ReportsPage
{
    private Panel parametersPanel = null!;
    private ComboBox reportTypeComboBox = null!;
    private DateTimePicker fromDatePicker = null!;
    private DateTimePicker toDatePicker = null!;
    private CheckBox completedCheckBox = null!;
    private Button generateButton = null!;
    private Button exportCsvButton = null!;
    private Button exportPdfButton = null!;
    private Label summaryLabel = null!;
    private DataGridView grid = null!;

    private void InitializeComponent()
    {
        parametersPanel = new Panel { Dock = DockStyle.Top, Height = 116, BackColor = UITheme.Surface, Padding = new Padding(14) };
        reportTypeComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = UITheme.BodyFont() };
        fromDatePicker = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(-30), Width = 140 };
        toDatePicker = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today, Width = 140 };
        completedCheckBox = new CheckBox { Text = "Только завершённые", Checked = true, AutoSize = true, Font = UITheme.BodyFont() };
        generateButton = UITheme.CreateButton("Сформировать", true, 145);
        exportCsvButton = UITheme.CreateButton("Экспорт CSV", false, 135);
        exportPdfButton = UITheme.CreateButton("Экспорт PDF", false, 135);
        summaryLabel = UITheme.CreateLabel("Параметры готовы к формированию отчёта", 9F, FontStyle.Regular, UITheme.TextMuted);
        grid = new DataGridView();
        ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
        SuspendLayout();
        var typeLabel = UITheme.CreateLabel("Тип отчёта", 8.5F, FontStyle.Regular, UITheme.TextMuted);
        typeLabel.Location = new Point(14, 8);
        reportTypeComboBox.SetBounds(14, 30, 290, 34);
        var fromLabel = UITheme.CreateLabel("Начало", 8.5F, FontStyle.Regular, UITheme.TextMuted);
        fromLabel.Location = new Point(320, 8);
        fromDatePicker.Location = new Point(320, 30);
        var toLabel = UITheme.CreateLabel("Конец", 8.5F, FontStyle.Regular, UITheme.TextMuted);
        toLabel.Location = new Point(476, 8);
        toDatePicker.Location = new Point(476, 30);
        completedCheckBox.Location = new Point(635, 34);
        generateButton.Location = new Point(14, 72);
        generateButton.Click += generateButton_Click;
        exportCsvButton.Location = new Point(170, 72);
        exportCsvButton.Click += exportCsvButton_Click;
        exportPdfButton.Location = new Point(315, 72);
        exportPdfButton.Click += exportPdfButton_Click;
        summaryLabel.Location = new Point(470, 80);
        parametersPanel.Controls.Add(typeLabel);
        parametersPanel.Controls.Add(reportTypeComboBox);
        parametersPanel.Controls.Add(fromLabel);
        parametersPanel.Controls.Add(fromDatePicker);
        parametersPanel.Controls.Add(toLabel);
        parametersPanel.Controls.Add(toDatePicker);
        parametersPanel.Controls.Add(completedCheckBox);
        parametersPanel.Controls.Add(generateButton);
        parametersPanel.Controls.Add(exportCsvButton);
        parametersPanel.Controls.Add(exportPdfButton);
        parametersPanel.Controls.Add(summaryLabel);
        UITheme.StyleGrid(grid);
        grid.Columns.Add("Name", "Показатель");
        grid.Columns.Add("Routes", "Маршруты");
        grid.Columns.Add("Deliveries", "Доставки");
        grid.Columns.Add("Rating", "Рейтинг");
        grid.Columns.Add("Amount", "Сумма");
        Content.Controls.Add(grid);
        Content.Controls.Add(parametersPanel);
        Name = "ReportsPage";
        ((System.ComponentModel.ISupportInitialize)grid).EndInit();
        ResumeLayout(false);
    }
}
