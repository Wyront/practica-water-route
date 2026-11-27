using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class CourierProblemsForm
{
    private DataGridView problemsGrid = null!;
    private Button createButton = null!;
    private Button refreshButton = null!;

    private void InitializeComponent()
    {
        problemsGrid = new DataGridView();
        UITheme.StyleGrid(problemsGrid);
        problemsGrid.Columns.Add("Date", "Дата");
        problemsGrid.Columns.Add("Route", "Маршрут");
        problemsGrid.Columns.Add("Category", "Категория");
        problemsGrid.Columns.Add("Status", "Статус");
        problemsGrid.Columns.Add("Resolution", "Решение");
        problemsGrid.Rows.Add("25.11.2026 09:20", "R-021", "Отсутствует товар", "В работе", "—");
        problemsGrid.Rows.Add("24.11.2026 18:10", "R-018", "Повреждение тары", "Решена", "Списание 1 бутыли");
        foreach (DataGridViewRow r in problemsGrid.Rows) r.Tag = new[] { r.Cells[0].Value?.ToString()!, r.Cells[1].Value?.ToString()! };

        createButton = UITheme.CreateButton("Зафиксировать проблему", true, 180);
        createButton.Click += createButton_Click;
        refreshButton = UITheme.CreateButton("Обновить  F5", false, 125);
        refreshButton.Click += (_, _) => RefreshData();

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = UITheme.Canvas
        };
        actions.Controls.AddRange(new Control[] { createButton, refreshButton });

        Content.Controls.Add(problemsGrid);
        Content.Controls.Add(actions);
        Content.Controls.SetChildIndex(actions, 0);
        Content.Controls.SetChildIndex(problemsGrid, 1);
    }
}