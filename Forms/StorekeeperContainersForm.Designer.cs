using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class StorekeeperContainersForm
{
    private DataGridView containersGrid = null!;
    private Button confirmButton = null!;
    private Button debtButton = null!;
    private Button reportButton = null!;
    private Button refreshButton = null!;

    private void InitializeComponent()
    {
        containersGrid = new DataGridView();
        UITheme.StyleGrid(containersGrid);
        containersGrid.Columns.Add("Route", "Маршрут");
        containersGrid.Columns.Add("Client", "Клиент");
        containersGrid.Columns.Add("Type", "Тип тары");
        containersGrid.Columns.Add("Issued", "Выдано");
        containersGrid.Columns.Add("Returned", "Возвращено");
        containersGrid.Columns.Add("Pending", "Ожидает");
        containersGrid.Columns.Add("Debt", "Долг");
        containersGrid.Columns.Add("Status", "Статус");
        containersGrid.Rows.Add("R-021", "Клиент №1", "Бутыль 19 л", "4", "0", "0", "4", "Все у клиента");
        containersGrid.Rows.Add("R-020", "Анна Воронова", "Бутыль 19 л", "6", "4", "2", "2", "Ожидает приёмку");
        containersGrid.Rows.Add("R-018", "Кафе «Север»", "Бутыль 19 л", "8", "8", "0", "0", "Возвращено");
        foreach (DataGridViewRow r in containersGrid.Rows) r.Tag = new[] { r.Cells[0].Value?.ToString()!, r.Cells[1].Value?.ToString()! };

        confirmButton = UITheme.CreateButton("Подтвердить приём", true, 160);
        confirmButton.Click += confirmButton_Click;
        debtButton = UITheme.CreateButton("Долги клиентов", false, 140);
        debtButton.Click += debtButton_Click;
        reportButton = UITheme.CreateButton("Отчёт по таре", false, 140);
        reportButton.Click += reportButton_Click;
        refreshButton = UITheme.CreateButton("Обновить  F5", false, 125);
        refreshButton.Click += (_, _) => RefreshData();

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = UITheme.Canvas
        };
        actions.Controls.AddRange(new Control[] { confirmButton, debtButton, reportButton, refreshButton });

        Content.Controls.Add(containersGrid);
        Content.Controls.Add(actions);
        Content.Controls.SetChildIndex(actions, 0);
        Content.Controls.SetChildIndex(containersGrid, 1);
    }
}