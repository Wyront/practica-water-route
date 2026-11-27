using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class DispatcherRouteForm
{
    private DataGridView routesGrid = null!;
    private Button editButton = null!;
    private Button viewButton = null!;
    private Button cancelRouteButton = null!;
    private Button requeueButton = null!;
    private Button refreshButton = null!;

    private void InitializeComponent()
    {
        routesGrid = new DataGridView();
        UITheme.StyleGrid(routesGrid);
        routesGrid.Columns.Add("Route", "Маршрут");
        routesGrid.Columns.Add("Order", "Заказ");
        routesGrid.Columns.Add("Date", "Дата");
        routesGrid.Columns.Add("Window", "Окно");
        routesGrid.Columns.Add("Warehouse", "Склад");
        routesGrid.Columns.Add("Courier", "Курьер");
        routesGrid.Columns.Add("Vehicle", "Автомобиль");
        routesGrid.Columns.Add("Status", "Статус");
        routesGrid.Rows.Add("R-021", "WR-10241", "28.11.2026", "12:00–15:00", "Центральный", "Курьер Алексей", "А001АА78", "Запланирован");
        routesGrid.Rows.Add("R-022", "WR-10242", "28.11.2026", "15:00–18:00", "Северный", "Курьер Сергей", "А002АА78", "Запланирован");
        routesGrid.Rows.Add("R-020", "WR-10240", "27.11.2026", "09:00–12:00", "Центральный", "Курьер Алексей", "А001АА78", "Завершён");
        foreach (DataGridViewRow r in routesGrid.Rows) r.Tag = new[] { r.Cells[0].Value?.ToString()!, r.Cells[1].Value?.ToString()!, r.Cells[2].Value?.ToString()!, r.Cells[3].Value?.ToString()!, r.Cells[4].Value?.ToString()!, r.Cells[5].Value?.ToString()!, r.Cells[6].Value?.ToString()!, r.Cells[7].Value?.ToString()! };

        editButton = UITheme.CreateButton("Редактировать назначение", true, 200);
        viewButton = UITheme.CreateButton("Просмотр / Карта", false, 160);
        cancelRouteButton = UITheme.CreateButton("Отменить маршрут", false, 160);
        cancelRouteButton.ForeColor = UITheme.Danger;
        requeueButton = UITheme.CreateButton("Вернуть заказ в очередь", false, 180);
        requeueButton.ForeColor = UITheme.Warning;
        refreshButton = UITheme.CreateButton("Обновить  F5", false, 125);

        editButton.Click += editButton_Click;
        viewButton.Click += viewButton_Click;
        cancelRouteButton.Click += cancelRouteButton_Click;
        requeueButton.Click += requeueButton_Click;
        refreshButton.Click += (_, _) => RefreshData();

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = UITheme.Canvas
        };
        actions.Controls.AddRange(new Control[] { editButton, viewButton, cancelRouteButton, requeueButton, refreshButton });

        Content.Controls.Add(routesGrid);
        Content.Controls.Add(actions);
        Content.Controls.SetChildIndex(actions, 0);
        Content.Controls.SetChildIndex(routesGrid, 1);
    }
}