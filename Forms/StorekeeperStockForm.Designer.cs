using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class StorekeeperStockForm
{
    private DataGridView stockGrid = null!;
    private Button lowStockButton = null!;
    private Button movementButton = null!;
    private Button receiptButton = null!;
    private Button adjustmentButton = null!;
    private Button journalButton = null!;
    private Button refreshButton = null!;
    private ComboBox warehouseFilter = null!;
    private TextBox searchTextBox = null!;

    private void InitializeComponent()
    {
        stockGrid = new DataGridView();
        UITheme.StyleGrid(stockGrid);
        stockGrid.Columns.Add("Sku", "Артикул");
        stockGrid.Columns.Add("Product", "Товар");
        stockGrid.Columns.Add("Warehouse", "Склад");
        stockGrid.Columns.Add("Physical", "Физический");
        stockGrid.Columns.Add("Reserved", "Резерв");
        stockGrid.Columns.Add("Available", "Доступно");
        stockGrid.Columns.Add("Min", "Минимум");
        stockGrid.Columns.Add("Status", "Статус");
        stockGrid.Rows.Add("WT-019", "Вода питьевая 19 л", "Центральный", "120", "36", "84", "50", "Достаточно");
        stockGrid.Rows.Add("WT-005", "Вода питьевая 5 л", "Центральный", "160", "34", "126", "40", "Достаточно");
        stockGrid.Rows.Add("AC-001", "Помпа механическая", "Центральный", "20", "8", "12", "10", "Низкий остаток");
        stockGrid.Rows.Add("WT-019", "Вода питьевая 19 л", "Северный", "80", "0", "80", "20", "Достаточно");
        stockGrid.Rows.Add("WT-005", "Вода питьевая 5 л", "Северный", "120", "0", "120", "30", "Достаточно");
        stockGrid.Rows.Add("AC-001", "Помпа механическая", "Северный", "15", "0", "15", "5", "Достаточно");
        stockGrid.Rows.Add("WT-019", "Вода питьевая 19 л", "Приморский", "60", "0", "60", "15", "Достаточно");
        stockGrid.Rows.Add("WT-005", "Вода питьевая 5 л", "Приморский", "90", "0", "90", "25", "Достаточно");
        stockGrid.Rows.Add("AC-001", "Помпа механическая", "Приморский", "10", "0", "10", "5", "Достаточно");
        foreach (DataGridViewRow r in stockGrid.Rows)
        {
            r.Tag = new[] { r.Cells[0].Value?.ToString()!, r.Cells[1].Value?.ToString()! };
            if (r.Cells[7].Value?.ToString() == "Низкий остаток")
                r.DefaultCellStyle.ForeColor = UITheme.Danger;
        }

        lowStockButton = UITheme.CreateButton("Низкие остатки", false, 140);
        lowStockButton.Click += lowStockButton_Click;
        movementButton = UITheme.CreateButton("Движение товара", false, 150);
        movementButton.Click += movementButton_Click;
        receiptButton = UITheme.CreateButton("Поступление", true, 130);
        receiptButton.Click += receiptButton_Click;
        adjustmentButton = UITheme.CreateButton("Корректировка", false, 130);
        adjustmentButton.Click += adjustmentButton_Click;
        journalButton = UITheme.CreateButton("Журнал движений", false, 140);
        journalButton.Click += journalButton_Click;
        refreshButton = UITheme.CreateButton("Обновить  F5", false, 125);
        refreshButton.Click += (_, _) => RefreshData();

        warehouseFilter = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = UITheme.BodyFont(), Width = 200 };
        warehouseFilter.Items.AddRange(new object[] { "Все склады", "Центральный", "Северный", "Приморский" });
        warehouseFilter.SelectedIndex = 0;

        searchTextBox = UITheme.CreateTextBox("Поиск по артикулу или названию");
        searchTextBox.Width = 300;

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = UITheme.Canvas
        };
        actions.Controls.AddRange(new Control[] { movementButton, receiptButton, adjustmentButton, journalButton, refreshButton });

        var filters = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = UITheme.Canvas };
        searchTextBox.SetBounds(0, 3, 300, 36);
        filters.Controls.Add(searchTextBox);
        var warehouseLabel = UITheme.CreateLabel("Склад:", 9.5F, FontStyle.Regular, UITheme.TextMuted);
        warehouseLabel.SetBounds(310, 10, 60, 28);
        filters.Controls.Add(warehouseLabel);
        warehouseFilter.SetBounds(376, 5, 200, 34);
        filters.Controls.Add(warehouseFilter);

        var statusActions = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, FlowDirection = FlowDirection.LeftToRight, BackColor = UITheme.Canvas };
        var lowStockBtn = UITheme.CreateButton("Низкие остатки", false, 140);
        lowStockBtn.Click += lowStockButton_Click;
        statusActions.Controls.Add(lowStockBtn);
        filters.Controls.Add(statusActions);

        Content.Controls.Add(stockGrid);
        Content.Controls.Add(filters);
        Content.Controls.Add(actions);
        Content.Controls.SetChildIndex(filters, 0);
        Content.Controls.SetChildIndex(actions, 1);
        Content.Controls.SetChildIndex(stockGrid, 2);
    }
}