using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class AdminReportsForm
{
    private ComboBox reportTypeComboBox = null!;
    private DateTimePicker fromDatePicker = null!;
    private DateTimePicker toDatePicker = null!;
    private CheckBox completedCheckBox = null!;
    private Button generateButton = null!;
    private Button exportCsvButton = null!;
    private Button exportPdfButton = null!;
    private DataGridView grid = null!;

    private void InitializeComponent()
    {
        var parameters = new Panel { Dock = DockStyle.Top, Height = 116, BackColor = UITheme.Surface, Padding = new Padding(14) };
        reportTypeComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = UITheme.BodyFont() };
        fromDatePicker = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(-30), Width = 140 };
        toDatePicker = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today, Width = 140 };
        completedCheckBox = new CheckBox { Text = "Только завершённые", Checked = true, AutoSize = true, Font = UITheme.BodyFont() };
        generateButton = UITheme.CreateButton("Сформировать", true, 145);
        exportCsvButton = UITheme.CreateButton("Экспорт CSV", false, 135);
        exportPdfButton = UITheme.CreateButton("Экспорт PDF", false, 135);
        grid = new DataGridView();

        ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
        SuspendLayout();

        var typeLabel = UITheme.CreateLabel("Тип отчёта", 8.5F, FontStyle.Regular, UITheme.TextMuted);
        typeLabel.Location = new Point(14, 8);
        reportTypeComboBox.SetBounds(14, 30, 350, 34);
        reportTypeComboBox.Items.AddRange(new object[]
        {
            "Заказы и выручка",
            "Выполнение маршрутов и недоставки",
            "Топ курьеров",
            "Складские остатки и движения",
            "Оборот и долг возвратной тары",
            "Зарегистрированные проблемы"
        });
        reportTypeComboBox.SelectedIndex = 0;

        var fromLabel = UITheme.CreateLabel("Начало", 8.5F, FontStyle.Regular, UITheme.TextMuted);
        fromLabel.Location = new Point(380, 8);
        fromDatePicker.Location = new Point(380, 30);
        var toLabel = UITheme.CreateLabel("Конец", 8.5F, FontStyle.Regular, UITheme.TextMuted);
        toLabel.Location = new Point(536, 8);
        toDatePicker.Location = new Point(536, 30);
        completedCheckBox.Location = new Point(695, 34);

        generateButton.Location = new Point(14, 72);
        generateButton.Click += generateButton_Click;
        exportCsvButton.Location = new Point(170, 72);
        exportCsvButton.Click += exportCsvButton_Click;
        exportPdfButton.Location = new Point(315, 72);
        exportPdfButton.Click += exportPdfButton_Click;

        parameters.Controls.Add(typeLabel);
        parameters.Controls.Add(reportTypeComboBox);
        parameters.Controls.Add(fromLabel);
        parameters.Controls.Add(fromDatePicker);
        parameters.Controls.Add(toLabel);
        parameters.Controls.Add(toDatePicker);
        parameters.Controls.Add(completedCheckBox);
        parameters.Controls.Add(generateButton);
        parameters.Controls.Add(exportCsvButton);
        parameters.Controls.Add(exportPdfButton);

        UITheme.StyleGrid(grid);
        grid.Columns.Add("Name", "Показатель");
        grid.Columns.Add("Value", "Значение");

        Content.Controls.Add(grid);
        Content.Controls.Add(parameters);
        Name = "AdminReportsForm";
        ((System.ComponentModel.ISupportInitialize)grid).EndInit();
        ResumeLayout(false);
    }
}