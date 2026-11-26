using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class RouteEditForm
{
    private TableLayoutPanel fields = null!;
    private TextBox orderTextBox = null!;
    private ComboBox warehouseComboBox = null!;
    private ComboBox courierComboBox = null!;
    private ComboBox vehicleComboBox = null!;
    private ComboBox dispatcherComboBox = null!;
    private DateTimePicker routeDatePicker = null!;
    private ComboBox timeWindowComboBox = null!;
    private ComboBox statusComboBox = null!;
    private Panel capacityPanel = null!;
    private Label capacityLabel = null!;
    private Label capacityHintLabel = null!;
    private Label noteLabel = null!;

    private void InitializeComponent()
    {
        fields = new TableLayoutPanel(); orderTextBox = new TextBox(); warehouseComboBox = new ComboBox(); courierComboBox = new ComboBox();
        vehicleComboBox = new ComboBox(); dispatcherComboBox = new ComboBox(); routeDatePicker = new DateTimePicker();
        timeWindowComboBox = new ComboBox(); statusComboBox = new ComboBox(); capacityPanel = new Panel();
        capacityLabel = new Label(); capacityHintLabel = new Label(); noteLabel = new Label();
        fields.SuspendLayout(); capacityPanel.SuspendLayout(); SuspendLayout();
        SetFormHeader("Назначение маршрута R-021", "Один заказ — один маршрут — одна остановка. Изменения доступны до начала выполнения.");
        ClientSize = new Size(790, 690);
        fields.Dock = DockStyle.Top; fields.Height = 430; fields.ColumnCount = 2; fields.RowCount = 8;
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190F)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        for (var i = 0; i < 8; i++) fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
        fields.Controls.Add(new Label { Text = "Заказ", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        orderTextBox.Dock = DockStyle.Fill; orderTextBox.ReadOnly = true; orderTextBox.Text = "WR-10241 · Клиент №1"; fields.Controls.Add(orderTextBox, 1, 0);
        AddComboRow(1, "Склад *", warehouseComboBox, ["Центральный склад", "Северный склад", "Приморский склад"]);
        AddComboRow(2, "Курьер *", courierComboBox, ["Курьер Алексей · свободен", "Курьер Сергей · свободен"]);
        AddComboRow(3, "Автомобиль *", vehicleComboBox, ["А001АА78 · Ford Transit · 100 мест", "А002АА78 · Ford Transit · 120 мест"]);
        AddComboRow(4, "Диспетчер *", dispatcherComboBox, ["Диспетчер Анна · свободна", "Диспетчер Игорь · свободен"]);
        fields.Controls.Add(new Label { Text = "Дата *", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 5);
        routeDatePicker.Dock = DockStyle.Fill; routeDatePicker.Format = DateTimePickerFormat.Short; routeDatePicker.Value = DateTime.Today.AddDays(1);
        fields.Controls.Add(routeDatePicker, 1, 5);
        AddComboRow(6, "Временное окно *", timeWindowComboBox, ["09:00–12:00", "12:00–15:00", "15:00–18:00"]);
        AddComboRow(7, "Статус", statusComboBox, ["Запланирован", "В работе", "Завершён", "Отменён"]);
        capacityPanel.Dock = DockStyle.Top; capacityPanel.Height = 80; capacityPanel.BackColor = UITheme.PrimarySoft; capacityPanel.Padding = new Padding(16);
        capacityLabel.Dock = DockStyle.Top; capacityLabel.Text = "Загрузка автомобиля: 11 условных мест из 100";
        capacityLabel.Font = UITheme.BodyFont(11F, FontStyle.Bold); capacityLabel.ForeColor = UITheme.Primary;
        capacityHintLabel.Dock = DockStyle.Top; capacityHintLabel.Text = "Проверяется только исходящий товар, без возвратной тары.";
        capacityHintLabel.Font = UITheme.BodyFont(9F); capacityHintLabel.ForeColor = UITheme.TextMuted;
        capacityPanel.Controls.Add(capacityHintLabel); capacityPanel.Controls.Add(capacityLabel);
        noteLabel.Dock = DockStyle.Bottom; noteLabel.Height = 60; noteLabel.Font = UITheme.BodyFont(9F); noteLabel.ForeColor = UITheme.TextMuted;
        noteLabel.Text = "После начала маршрута назначение блокируется. При смене склада резерв переносится, а распределение повторяется.";
        contentPanel.Controls.Add(noteLabel); contentPanel.Controls.Add(capacityPanel); contentPanel.Controls.Add(fields);
        fields.ResumeLayout(false); fields.PerformLayout(); capacityPanel.ResumeLayout(false); ResumeLayout(false);
    }

    private void AddComboRow(int row, string caption, ComboBox comboBox, string[] items)
    {
        fields.Controls.Add(new Label { Text = caption, AutoSize = true, Anchor = AnchorStyles.Left }, 0, row);
        comboBox.Dock = DockStyle.Fill; comboBox.DropDownStyle = ComboBoxStyle.DropDownList; comboBox.Items.AddRange(items); comboBox.SelectedIndex = 0;
        fields.Controls.Add(comboBox, 1, row);
    }
}
