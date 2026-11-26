using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

partial class ProblemEditForm
{
    private TableLayoutPanel fields = null!;
    private ComboBox targetTypeComboBox = null!;
    private ComboBox targetComboBox = null!;
    private ComboBox categoryComboBox = null!;
    private ComboBox statusComboBox = null!;
    private TextBox descriptionTextBox = null!;
    private TextBox resolutionTextBox = null!;
    private Label noteLabel = null!;

    private void InitializeComponent()
    {
        fields = FormLayout.CreateTwoColumnLayout();
        targetTypeComboBox = FormLayout.CreateComboBox("Маршрут", "Заказ", "Доставка");
        targetComboBox = FormLayout.CreateComboBox("R-021 · WR-10241", "R-022 · WR-10242");
        categoryComboBox = FormLayout.CreateComboBox(
            "Отсутствует товар", "Клиент не отвечает", "Повреждение тары", "Автомобиль", "Адрес", "Другое");
        statusComboBox = FormLayout.CreateComboBox("Открыта", "В работе", "Решена");
        descriptionTextBox = UITheme.CreateTextBox(string.Empty, true);
        resolutionTextBox = UITheme.CreateTextBox(string.Empty, true);
        noteLabel = UITheme.CreateLabel(string.Empty, 9F, FontStyle.Regular, UITheme.TextMuted, false);
        fields.SuspendLayout();
        SuspendLayout();
        //
        // fields
        //
        FormLayout.AddRow(fields, "Тип объекта *", targetTypeComboBox, 52);
        FormLayout.AddRow(fields, "Объект *", targetComboBox, 52);
        FormLayout.AddRow(fields, "Категория *", categoryComboBox, 52);
        FormLayout.AddRow(fields, "Статус", statusComboBox, 52);
        FormLayout.AddRow(fields, "Описание *", descriptionTextBox, 115);
        FormLayout.AddRow(fields, "Комментарий решения", resolutionTextBox, 115);
        //
        // noteLabel
        //
        noteLabel.Dock = DockStyle.Bottom;
        noteLabel.Height = 60;
        noteLabel.Name = "noteLabel";
        noteLabel.Text = "Курьер регистрирует проблему, диспетчер обрабатывает её, администратор просматривает.";
        //
        // ProblemEditForm
        //
        Content.Controls.Add(noteLabel);
        Content.Controls.Add(fields);
        Name = "ProblemEditForm";
        fields.ResumeLayout(false);
        fields.PerformLayout();
        ResumeLayout(false);
    }

    private void InitializeComponent(UserRole role, bool readOnly)
    {
        InitializeComponent();
        SaveButton.Visible = !readOnly && role != UserRole.Admin;
    }
}
