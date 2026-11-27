using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed class CourierHistoryForm : BaseEditForm
{
    public CourierHistoryForm() : base("История маршрутов", "Выполненные маршруты, статистика и рейтинг", 1100, 700)
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        var grid = new DataGridView();
        UITheme.StyleGrid(grid);
        grid.Columns.Add("Route", "Маршрут");
        grid.Columns.Add("Date", "Дата");
        grid.Columns.Add("Order", "Заказ");
        grid.Columns.Add("Status", "Результат");
        grid.Columns.Add("Rating", "Оценка");
        grid.Rows.Add("R-020", "27.11.2026", "WR-10240", "Доставлен", "5 ★");
        grid.Rows.Add("R-018", "26.11.2026", "WR-10238", "Доставлен", "5 ★");
        grid.Rows.Add("R-018", "26.11.2026", "WR-10238", "Не доставлен", "—");
        Content.Controls.Add(grid);

        var stats = new Panel { Dock = DockStyle.Bottom, Height = 120, BackColor = UITheme.Surface, Padding = new Padding(14) };
        var title = UITheme.CreateLabel("Статистика курьера", 12F, FontStyle.Bold);
        title.Dock = DockStyle.Top;
        stats.Controls.Add(title);
        var details = UITheme.CreateLabel("Всего доставок: 42  ·  Недоставок: 1  ·  Средний рейтинг: 4.8 ★  ·  Возврат тары: 95%", 10F, FontStyle.Regular);
        details.Dock = DockStyle.Top;
        stats.Controls.Add(details);
        Content.Controls.Add(stats);
    }
}