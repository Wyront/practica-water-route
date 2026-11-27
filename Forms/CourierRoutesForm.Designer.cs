using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class CourierRoutesForm
{
    private DataGridView routesGrid = null!;
    private Button openRouteButton = null!;
    private Button startRouteButton = null!;
    private Button openChatButton = null!;
    private Button historyButton = null!;
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
        routesGrid.Columns.Add("Vehicle", "Автомобиль");
        routesGrid.Columns.Add("Progress", "Прогресс");
        routesGrid.Columns.Add("Status", "Статус");
        routesGrid.Rows.Add("R-021", "WR-10241", "28.11.2026", "12:00–15:00", "Центральный", "А001АА78", "0/1", "Запланирован");
        routesGrid.Rows.Add("R-022", "WR-10242", "28.11.2026", "15:00–18:00", "Северный", "А002АА78", "0/1", "Запланирован");
        routesGrid.Rows.Add("R-020", "WR-10240", "27.11.2026", "09:00–12:00", "Центральный", "А001АА78", "1/1", "Завершён");
        foreach (DataGridViewRow r in routesGrid.Rows) r.Tag = new[] { r.Cells[0].Value?.ToString()!, r.Cells[1].Value?.ToString()!, r.Cells[2].Value?.ToString()!, r.Cells[3].Value?.ToString()!, r.Cells[4].Value?.ToString()!, r.Cells[5].Value?.ToString()!, r.Cells[6].Value?.ToString()!, r.Cells[7].Value?.ToString()! };

        openRouteButton = UITheme.CreateButton("Открыть маршрут", false, 150);
        openRouteButton.Click += openRouteButton_Click;
        startRouteButton = UITheme.CreateButton("Начать маршрут", true, 140);
        startRouteButton.Click += startRouteButton_Click;
        openChatButton = UITheme.CreateButton("Чат с диспетчером", false, 160);
        openChatButton.Click += openChatButton_Click;
        historyButton = UITheme.CreateButton("История маршрутов", false, 150);
        historyButton.Click += historyButton_Click;
        refreshButton = UITheme.CreateButton("Обновить  F5", false, 125);
        refreshButton.Click += (_, _) => RefreshData();

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = UITheme.Canvas
        };
        actions.Controls.AddRange(new Control[] { openRouteButton, startRouteButton, openChatButton, historyButton, refreshButton });

        Content.Controls.Add(routesGrid);
        Content.Controls.Add(actions);
        Content.Controls.SetChildIndex(actions, 0);
        Content.Controls.SetChildIndex(routesGrid, 1);
    }
}