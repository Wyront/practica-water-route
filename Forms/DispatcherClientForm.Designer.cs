using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class DispatcherClientForm
{
    private DataGridView clientsGrid = null!;
    private Button createButton = null!;
    private Button editButton = null!;
    private Button addressesButton = null!;
    private Button historyButton = null!;
    private Button callButton = null!;
    private Button refreshButton = null!;

    private void InitializeComponent()
    {
        clientsGrid = new DataGridView();
        UITheme.StyleGrid(clientsGrid);
        clientsGrid.Columns.Add("Id", "ID");
        clientsGrid.Columns.Add("Name", "Клиент");
        clientsGrid.Columns.Add("Phone", "Телефон");
        clientsGrid.Columns.Add("Addresses", "Адресов");
        clientsGrid.Columns.Add("Orders", "Заказов");
        clientsGrid.Columns.Add("Active", "Активен");
        clientsGrid.Rows.Add("1", "Клиент №1", "+79000000011", "2", "8", "Да");
        clientsGrid.Rows.Add("2", "ООО «Ручеёк»", "+79000000012", "1", "5", "Да");
        clientsGrid.Rows.Add("3", "Кафе «Нева»", "+79000000013", "3", "12", "Да");
        foreach (DataGridViewRow r in clientsGrid.Rows) r.Tag = new[] { r.Cells[0].Value?.ToString()!, r.Cells[1].Value?.ToString()!, r.Cells[2].Value?.ToString()!, r.Cells[3].Value?.ToString()!, r.Cells[4].Value?.ToString()!, r.Cells[5].Value?.ToString()! };

        createButton = UITheme.CreateButton("Создать клиента", true, 160);
        editButton = UITheme.CreateButton("Редактировать", false, 140);
        addressesButton = UITheme.CreateButton("Адреса", false, 120);
        historyButton = UITheme.CreateButton("История заказов", false, 160);
        callButton = UITheme.CreateButton("Позвонить", false, 130);
        refreshButton = UITheme.CreateButton("Обновить  F5", false, 125);

        createButton.Click += createButton_Click;
        editButton.Click += editButton_Click;
        addressesButton.Click += addressesButton_Click;
        historyButton.Click += historyButton_Click;
        callButton.Click += callButton_Click;
        refreshButton.Click += (_, _) => RefreshData();

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = UITheme.Canvas
        };
        actions.Controls.AddRange(new Control[] { createButton, editButton, addressesButton, historyButton, callButton, refreshButton });

        Content.Controls.Add(clientsGrid);
        Content.Controls.Add(actions);
    }
}