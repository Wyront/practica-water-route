using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class ClientOrderHistoryForm
{
    private DataGridView ordersGrid = null!;
    private Button repeatButton = null!;
    private Button cancelButton = null!;
    private Button viewButton = null!;
    private Button pdfButton = null!;
    private Button refreshButton = null!;

    private void InitializeComponent()
    {
        ordersGrid = new DataGridView();
        UITheme.StyleGrid(ordersGrid);
        ordersGrid.Columns.Add("Id", "ID");
        ordersGrid.Columns.Add("Number", "Заказ");
        ordersGrid.Columns.Add("Date", "Дата");
        ordersGrid.Columns.Add("Address", "Адрес");
        ordersGrid.Columns.Add("Warehouse", "Склад");
        ordersGrid.Columns.Add("Status", "Статус");
        ordersGrid.Columns.Add("Amount", "Сумма");
        ordersGrid.Rows.Add("1", "WR-10241", "28.11.2026", "Невский проспект, 10", "Центральный", "Запланирован", "1 990 ₽");
        ordersGrid.Rows.Add("2", "WR-10239", "27.11.2026", "Невский проспект, 10", "Центральный", "Доставлен", "2 450 ₽");
        ordersGrid.Rows.Add("3", "WR-10238", "26.11.2026", "Приморский проспект, 25", "Северный", "Доставлен", "980 ₽");
        foreach (DataGridViewRow r in ordersGrid.Rows) r.Tag = new[] { r.Cells[0].Value?.ToString()!, r.Cells[1].Value?.ToString()!, r.Cells[2].Value?.ToString()!, r.Cells[3].Value?.ToString()!, r.Cells[4].Value?.ToString()!, r.Cells[5].Value?.ToString()!, r.Cells[6].Value?.ToString()! };

        repeatButton = UITheme.CreateButton("Повторить заказ", true, 160);
        cancelButton = UITheme.CreateButton("Отменить", false, 120);
        cancelButton.ForeColor = UITheme.Danger;
        viewButton = UITheme.CreateButton("Просмотр", false, 120);
        pdfButton = UITheme.CreateButton("PDF заказа", false, 130);
        refreshButton = UITheme.CreateButton("Обновить  F5", false, 125);

        repeatButton.Click += repeatOrderButton_Click;
        cancelButton.Click += cancelOrderButton_Click;
        viewButton.Click += viewOrderButton_Click;
        pdfButton.Click += pdfButton_Click;
        refreshButton.Click += (_, _) => RefreshData();

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = UITheme.Canvas
        };
        actions.Controls.AddRange(new Control[] { repeatButton, cancelButton, viewButton, pdfButton, refreshButton });

        Content.Controls.Add(ordersGrid);
        Content.Controls.Add(actions);
    }
}