using WaterRoute.Infrastructure;

namespace WaterRoute.Controls;

partial class ImportPage
{
    private Panel settingsPanel = null!;
    private Label typeLabel = null!;
    private ComboBox typeComboBox = null!;
    private Button templateButton = null!;
    private Label pathLabel = null!;
    private TextBox pathTextBox = null!;
    private Button browseButton = null!;
    private Button previewButton = null!;
    private DataGridView previewGrid = null!;
    private DataGridViewTextBoxColumn lineColumn = null!;
    private DataGridViewTextBoxColumn warehouseColumn = null!;
    private DataGridViewTextBoxColumn skuColumn = null!;
    private DataGridViewTextBoxColumn quantityColumn = null!;
    private DataGridViewTextBoxColumn resultColumn = null!;
    private FlowLayoutPanel actionsPanel = null!;
    private Button importButton = null!;
    private Button errorsButton = null!;
    private Label statusLabel = null!;

    private void InitializeComponent()
    {
        settingsPanel = new Panel();
        typeLabel = new Label();
        typeComboBox = new ComboBox();
        templateButton = new Button();
        pathLabel = new Label();
        pathTextBox = new TextBox();
        browseButton = new Button();
        previewButton = new Button();
        previewGrid = new DataGridView();
        lineColumn = new DataGridViewTextBoxColumn();
        warehouseColumn = new DataGridViewTextBoxColumn();
        skuColumn = new DataGridViewTextBoxColumn();
        quantityColumn = new DataGridViewTextBoxColumn();
        resultColumn = new DataGridViewTextBoxColumn();
        actionsPanel = new FlowLayoutPanel();
        importButton = new Button();
        errorsButton = new Button();
        statusLabel = new Label();
        settingsPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)previewGrid).BeginInit();
        actionsPanel.SuspendLayout();
        SuspendLayout();
        // 
        // settingsPanel
        // 
        settingsPanel.BackColor = UITheme.Surface;
        settingsPanel.Controls.Add(typeLabel);
        settingsPanel.Controls.Add(typeComboBox);
        settingsPanel.Controls.Add(templateButton);
        settingsPanel.Controls.Add(pathLabel);
        settingsPanel.Controls.Add(pathTextBox);
        settingsPanel.Controls.Add(browseButton);
        settingsPanel.Controls.Add(previewButton);
        settingsPanel.Dock = DockStyle.Top;
        settingsPanel.Height = 142;
        settingsPanel.Name = "settingsPanel";
        // 
        // typeLabel
        // 
        typeLabel.AutoSize = true;
        typeLabel.Location = new Point(14, 8);
        typeLabel.Name = "typeLabel";
        typeLabel.Text = "Тип импорта";
        // 
        // typeComboBox
        // 
        typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        typeComboBox.Items.AddRange(new object[] { "Складские поступления", "Товары" });
        typeComboBox.Name = "typeComboBox";
        typeComboBox.SelectedIndex = 0;
        typeComboBox.SetBounds(14, 30, 280, 34);
        // 
        // templateButton
        // 
        templateButton.FlatStyle = FlatStyle.Flat;
        templateButton.Name = "templateButton";
        templateButton.SetBounds(310, 28, 160, 38);
        templateButton.Text = "Скачать шаблон";
        // 
        // pathLabel
        // 
        pathLabel.AutoSize = true;
        pathLabel.Location = new Point(14, 76);
        pathLabel.Name = "pathLabel";
        pathLabel.Text = "CSV-файл";
        // 
        // pathTextBox
        // 
        pathTextBox.Name = "pathTextBox";
        pathTextBox.PlaceholderText = "Файл не выбран";
        pathTextBox.SetBounds(14, 98, 620, 34);
        // 
        // browseButton
        // 
        browseButton.FlatStyle = FlatStyle.Flat;
        browseButton.Name = "browseButton";
        browseButton.SetBounds(650, 94, 110, 38);
        browseButton.Text = "Обзор…";
        browseButton.Click += browseButton_Click;
        // 
        // previewButton
        // 
        previewButton.BackColor = UITheme.Primary;
        previewButton.FlatStyle = FlatStyle.Flat;
        previewButton.ForeColor = Color.White;
        previewButton.Name = "previewButton";
        previewButton.SetBounds(778, 94, 160, 38);
        previewButton.Text = "Проверить файл";
        previewButton.Click += previewButton_Click;
        // 
        // previewGrid
        // 
        UITheme.StyleGrid(previewGrid);
        previewGrid.Columns.AddRange(new DataGridViewColumn[]
        {
            lineColumn,
            warehouseColumn,
            skuColumn,
            quantityColumn,
            resultColumn
        });
        previewGrid.Name = "previewGrid";
        // 
        // lineColumn
        // 
        lineColumn.HeaderText = "Строка";
        lineColumn.Name = "Line";
        // 
        // warehouseColumn
        // 
        warehouseColumn.HeaderText = "Склад";
        warehouseColumn.Name = "Warehouse";
        // 
        // skuColumn
        // 
        skuColumn.HeaderText = "Артикул";
        skuColumn.Name = "Sku";
        // 
        // quantityColumn
        // 
        quantityColumn.HeaderText = "Количество";
        quantityColumn.Name = "Quantity";
        // 
        // resultColumn
        // 
        resultColumn.HeaderText = "Результат проверки";
        resultColumn.Name = "Result";
        // 
        // actionsPanel
        // 
        actionsPanel.Controls.Add(importButton);
        actionsPanel.Controls.Add(errorsButton);
        actionsPanel.Controls.Add(statusLabel);
        actionsPanel.Dock = DockStyle.Bottom;
        actionsPanel.Height = 58;
        actionsPanel.Name = "actionsPanel";
        // 
        // importButton
        // 
        importButton.BackColor = UITheme.Primary;
        importButton.FlatStyle = FlatStyle.Flat;
        importButton.ForeColor = Color.White;
        importButton.Height = 38;
        importButton.Name = "importButton";
        importButton.Text = "Импортировать одной транзакцией";
        importButton.Width = 265;
        // 
        // errorsButton
        // 
        errorsButton.Name = "errorsButton";
        errorsButton.Text = "Отчёт об ошибках";
        errorsButton.Width = 170;
        // 
        // statusLabel
        // 
        statusLabel.AutoSize = true;
        statusLabel.ForeColor = UITheme.TextMuted;
        statusLabel.Margin = new Padding(12, 8, 0, 0);
        statusLabel.Name = "statusLabel";
        statusLabel.Text = "Выберите CSV-файл для проверки";
        // 
        // ImportPage
        // 
        Content.Controls.Add(previewGrid);
        Content.Controls.Add(actionsPanel);
        Content.Controls.Add(settingsPanel);
        Name = "ImportPage";
        Size = new Size(1000, 700);
        settingsPanel.ResumeLayout(false);
        settingsPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)previewGrid).EndInit();
        actionsPanel.ResumeLayout(false);
        actionsPanel.PerformLayout();
        ResumeLayout(false);
    }
}
