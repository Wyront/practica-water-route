using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class DispatcherOrderForm
{
    private DataGridView ordersGrid = null!;
    private Button createButton = null!;
    private Button editButton = null!;
    private Button changeWarehouseButton = null!;
    private Button reassignButton = null!;
    private Button cancelButton = null!;
    private Button processQueueButton = null!;
    private Button refreshButton = null!;
    private ComboBox statusFilter = null!;
    private TextBox searchTextBox = null!;

    private void InitializeComponent()
    {
        ordersGrid = new DataGridView();
        UITheme.StyleGrid(ordersGrid);
        ordersGrid.Columns.Add("Id", "ID");
        ordersGrid.Columns.Add("Number", "Заказ");
        ordersGrid.Columns.Add("Client", "Клиент");
        ordersGrid.Columns.Add("Address", "Адрес");
        ordersGrid.Columns.Add("Warehouse", "Склад");
        ordersGrid.Columns.Add("Status", "Статус");
        ordersGrid.Columns.Add("Window", "Окно");
        ordersGrid.Columns.Add("Courier", "Курьер");
        ordersGrid.Rows.Add("1", "WR-10241", "Клиент №1", "Невский проспект, 10", "Центральный", "Запланирован", "12:00–15:00", "Курьер Алексей");
        ordersGrid.Rows.Add("2", "WR-10242", "Анна Воронова", "Приморский проспект, 25", "Северный", "Новый", "—", "—");
        ordersGrid.Rows.Add("3", "WR-10243", "Кафе «Север»", "ул. Озёрная, 4", "Приморский", "Доставлен", "09:00–12:00", "Курьер Сергей");
        foreach (DataGridViewRow r in ordersGrid.Rows) r.Tag = new[] { r.Cells[0].Value?.ToString()!, r.Cells[1].Value?.ToString()!, r.Cells[2].Value?.ToString()!, r.Cells[3].Value?.ToString()!, r.Cells[4].Value?.ToString()!, r.Cells[5].Value?.ToString()!, r.Cells[6].Value?.ToString()!, r.Cells[7].Value?.ToString()! };

        createButton = UITheme.CreateButton("Создать заказ", true, 140);
        editButton = UITheme.CreateButton("Редактировать", false, 140);
        changeWarehouseButton = UITheme.CreateButton("Сменить склад", false, 145);
        reassignButton = UITheme.CreateButton("Переназначить", false, 140);
        cancelButton = UITheme.CreateButton("Отменить", false, 120);
        cancelButton.ForeColor = UITheme.Danger;
        processQueueButton = UITheme.CreateButton("Обработать очередь", true, 170);
        refreshButton = UITheme.CreateButton("Обновить  F5", false, 125);

        statusFilter = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = UITheme.BodyFont(), Width = 160 };
        statusFilter.Items.AddRange(new object[] { "Все", "Новый", "Запланирован", "В доставке", "Доставлен", "Не доставлен", "Отменён" });
        statusFilter.SelectedIndex = 0;

        searchTextBox = UITheme.CreateTextBox("Поиск по номеру или клиенту");
        searchTextBox.Width = 300;

        createButton.Click += createButton_Click;
        editButton.Click += editButton_Click;
        changeWarehouseButton.Click += changeWarehouseButton_Click;
        reassignButton.Click += reassignButton_Click;
        cancelButton.Click += cancelButton_Click;
        processQueueButton.Click += processQueueButton_Click;
        refreshButton.Click += (_, _) => RefreshData();

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = UITheme.Canvas
        };
        actions.Controls.AddRange(new Control[] { createButton, editButton, changeWarehouseButton, reassignButton, cancelButton, processQueueButton, refreshButton });

        var filters = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = UITheme.Canvas };
        searchTextBox.SetBounds(0, 3, 300, 36);
        filters.Controls.Add(searchTextBox);
        var statusLabel = UITheme.CreateLabel("Статус:", 9.5F, FontStyle.Regular, UITheme.TextMuted);
        statusLabel.SetBounds(310, 10, 60, 28);
        filters.Controls.Add(statusLabel);
        statusFilter.SetBounds(376, 5, 160, 34);
        filters.Controls.Add(statusFilter);

        Content.Controls.Add(ordersGrid);
        Content.Controls.Add(filters);
        Content.Controls.Add(actions);
        Content.Controls.SetChildIndex(ordersGrid, 2);
        Content.Controls.SetChildIndex(filters, 1);
        Content.Controls.SetChildIndex(actions, 0);
    }
}