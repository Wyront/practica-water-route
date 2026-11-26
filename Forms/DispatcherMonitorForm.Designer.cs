using WaterRoute.Forms;
using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class DispatcherMonitorForm
{
    private DataGridView routesGrid = null!;
    private DataGridView eventsGrid = null!;
    private Button openRouteButton = null!;
    private Button openChatButton = null!;
    private Button problemButton = null!;
    private Button callClientButton = null!;
    private Button refreshButton = null!;

    private void InitializeComponent()
    {
        var tables = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, BackColor = UITheme.Canvas };
        tables.RowStyles.Add(new RowStyle(SizeType.Percent, 65));
        tables.RowStyles.Add(new RowStyle(SizeType.Percent, 35));

        routesGrid = new DataGridView();
        UITheme.StyleGrid(routesGrid);
        routesGrid.Columns.Add("Route", "Маршрут");
        routesGrid.Columns.Add("Order", "Заказ");
        routesGrid.Columns.Add("Courier", "Курьер");
        routesGrid.Columns.Add("Vehicle", "Автомобиль");
        routesGrid.Columns.Add("Progress", "Статус");
        routesGrid.Columns.Add("Event", "Последнее событие");
        routesGrid.Rows.Add("R-021", "WR-10241", "Курьер Алексей", "А001АА78", "Курьер прибыл", "12:15");
        routesGrid.Rows.Add("R-022", "WR-10242", "Курьер Сергей", "А002АА78", "Запланирован", "—");
        foreach (DataGridViewRow r in routesGrid.Rows) r.Tag = new[] { r.Cells[0].Value?.ToString()!, r.Cells[1].Value?.ToString()!, r.Cells[2].Value?.ToString()!, r.Cells[3].Value?.ToString()!, r.Cells[4].Value?.ToString()!, r.Cells[5].Value?.ToString()! };

        eventsGrid = new DataGridView();
        UITheme.StyleGrid(eventsGrid);
        eventsGrid.Columns.Add("Time", "Время");
        eventsGrid.Columns.Add("Event", "Событие");
        eventsGrid.Rows.Add("12:15", "R-021 · курьер прибыл по адресу");
        eventsGrid.Rows.Add("12:10", "R-021 · курьер начал маршрут");
        eventsGrid.Rows.Add("11:55", "WR-10242 · маршрут назначен");
        eventsGrid.Rows.Add("11:40", "Вода 19 л · низкий остаток на Северном складе");

        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, FlowDirection = FlowDirection.LeftToRight };
        var refreshToolbar = UITheme.CreateButton("Обновить  F5", true, 125);
        refreshToolbar.Click += (_, _) => RefreshData();
        toolbar.Controls.Add(refreshToolbar);
        var openRoute = UITheme.CreateButton("Открыть маршрут", false, 150);
        openRoute.Click += openRouteButton_Click;
        toolbar.Controls.Add(openRoute);
        var openChat = UITheme.CreateButton("Открыть чат", false, 140);
        openChat.Click += openChatButton_Click;
        toolbar.Controls.Add(openChat);
        var problem = UITheme.CreateButton("Зафиксировать проблему", false, 180);
        problem.Click += problemButton_Click;
        toolbar.Controls.Add(problem);
        var call = UITheme.CreateButton("Позвонить клиенту", false, 160);
        call.Click += callClientButton_Click;
        toolbar.Controls.Add(call);

        var eventsPanel = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.Surface, Padding = new Padding(12) };
        var eventsTitle = UITheme.CreateLabel("Последние события и проблемы", 11F, FontStyle.Bold);
        eventsTitle.Dock = DockStyle.Top;
        eventsPanel.Controls.Add(eventsGrid);
        eventsPanel.Controls.Add(eventsTitle);

        tables.Controls.Add(routesGrid, 0, 0);
        tables.Controls.Add(eventsPanel, 0, 1);

        var actions = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 50, FlowDirection = FlowDirection.LeftToRight };
        var refreshBottom = UITheme.CreateButton("Обновить  F5", true, 125);
        refreshBottom.Click += (_, _) => RefreshData();
        actions.Controls.Add(refreshBottom);

        Content.Controls.Add(tables);
        Content.Controls.Add(toolbar);
        Content.Controls.Add(actions);
        Content.Controls.SetChildIndex(toolbar, 0);
        Content.Controls.SetChildIndex(tables, 1);
        Content.Controls.SetChildIndex(actions, 2);
    }
}