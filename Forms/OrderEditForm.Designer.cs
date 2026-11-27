using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class OrderEditForm
{
    private TableLayoutPanel fields = null!;
    private ComboBox clientComboBox = null!;
    private ComboBox addressComboBox = null!;
    private ComboBox warehouseComboBox = null!;
    private ComboBox paymentComboBox = null!;
    private TextBox commentTextBox = null!;
    private FlowLayoutPanel itemsToolbar = null!;
    private Button addItemButton = null!;
    private Button removeItemButton = null!;
    private Button checkStockButton = null!;
    private DataGridView itemsGrid = null!;
    private Panel totalPanel = null!;
    private Label reserveLabel = null!;
    private Label totalLabel = null!;

    private void InitializeComponent()
    {
        fields = new TableLayoutPanel(); clientComboBox = new ComboBox(); addressComboBox = new ComboBox(); warehouseComboBox = new ComboBox();
        paymentComboBox = new ComboBox(); commentTextBox = new TextBox(); itemsToolbar = new FlowLayoutPanel();
        addItemButton = new Button(); removeItemButton = new Button(); checkStockButton = new Button(); itemsGrid = new DataGridView();
        totalPanel = new Panel(); reserveLabel = new Label(); totalLabel = new Label();
        fields.SuspendLayout(); itemsToolbar.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)itemsGrid).BeginInit(); totalPanel.SuspendLayout(); SuspendLayout();
        SetFormHeader("Новый заказ", "Один заказ обслуживается одним складом. Остатки проверяются перед созданием резерва.");
        ClientSize = new Size(1040, 800);
        fields.Dock = DockStyle.Top; fields.Height = 300; fields.ColumnCount = 2; fields.RowCount = 5;
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        for (var i = 0; i < 4; i++) fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F)); fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
        AddComboRow(0, "Клиент *", clientComboBox, ["Клиент №1", "ООО «Ручеёк»", "Кафе «Нева»"]);
        AddComboRow(1, "Адрес *", addressComboBox, ["Невский проспект, 10 · Центральный склад", "Приморский проспект, 25 · Северный склад"]);
        AddComboRow(2, "Склад *", warehouseComboBox, ["Центральный склад", "Северный склад", "Приморский склад"]);
        AddComboRow(3, "Способ оплаты *", paymentComboBox, ["Наличные", "Карта", "Онлайн", "Безнал"]);
        fields.Controls.Add(new Label { Text = "Комментарий", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 4);
        commentTextBox.Dock = DockStyle.Fill; commentTextBox.Multiline = true; commentTextBox.PlaceholderText = "Пожелание к дате и времени доставки";
        fields.Controls.Add(commentTextBox, 1, 4);
        itemsToolbar.Dock = DockStyle.Top; itemsToolbar.Height = 50; itemsToolbar.WrapContents = false;
        ConfigureButton(addItemButton, "Добавить товар", 145, true); ConfigureButton(removeItemButton, "Удалить позицию", 145, false);
        ConfigureButton(checkStockButton, "Проверить остатки", 165, false); checkStockButton.Click += checkStockButton_Click;
        itemsToolbar.Controls.Add(addItemButton); itemsToolbar.Controls.Add(removeItemButton); itemsToolbar.Controls.Add(checkStockButton);
        UITheme.StyleGrid(itemsGrid);
        itemsGrid.Columns.Add("Product", "Товар"); itemsGrid.Columns.Add("Available", "Доступно"); itemsGrid.Columns.Add("Quantity", "Количество");
        itemsGrid.Columns.Add("Price", "Цена"); itemsGrid.Columns.Add("Total", "Сумма");
        itemsGrid.Rows.Add("Вода питьевая 19 л", "84", "4", "250,00 ₽", "1 000,00 ₽");
        itemsGrid.Rows.Add("Вода питьевая 5 л", "126", "6", "90,00 ₽", "540,00 ₽");
        itemsGrid.Rows.Add("Помпа механическая", "12", "1", "450,00 ₽", "450,00 ₽");
        totalPanel.Dock = DockStyle.Bottom; totalPanel.Height = 54; totalPanel.BackColor = UITheme.SurfaceMuted; totalPanel.Padding = new Padding(12);
        reserveLabel.Dock = DockStyle.Left; reserveLabel.Width = 500; reserveLabel.Text = "Резерв создаётся после проверки всех позиций";
        reserveLabel.Font = UITheme.BodyFont(9F); reserveLabel.ForeColor = UITheme.TextMuted;
        totalLabel.Dock = DockStyle.Right; totalLabel.Width = 280; totalLabel.Text = "Итого: 1 990,00 ₽";
        totalLabel.Font = UITheme.TitleFont(14F); totalLabel.ForeColor = UITheme.Primary; totalLabel.TextAlign = ContentAlignment.MiddleRight;
        totalPanel.Controls.Add(totalLabel); totalPanel.Controls.Add(reserveLabel);
        contentPanel.Controls.Add(itemsGrid); contentPanel.Controls.Add(totalPanel); contentPanel.Controls.Add(itemsToolbar); contentPanel.Controls.Add(fields);
        fields.ResumeLayout(false); fields.PerformLayout(); itemsToolbar.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)itemsGrid).EndInit();
        totalPanel.ResumeLayout(false); ResumeLayout(false);
    }

    private void AddComboRow(int row, string caption, ComboBox comboBox, string[] items)
    {
        fields.Controls.Add(new Label { Text = caption, AutoSize = true, Anchor = AnchorStyles.Left }, 0, row);
        comboBox.Dock = DockStyle.Fill; comboBox.DropDownStyle = ComboBoxStyle.DropDownList; comboBox.Items.AddRange(items); comboBox.SelectedIndex = 0;
        fields.Controls.Add(comboBox, 1, row);
    }

    private static void ConfigureButton(Button button, string text, int width, bool primary)
    {
        button.Text = text; button.Width = width; button.Height = 38; button.BackColor = primary ? UITheme.Primary : UITheme.Surface;
        button.ForeColor = primary ? Color.White : UITheme.Text; button.FlatStyle = FlatStyle.Flat;
    }
}
