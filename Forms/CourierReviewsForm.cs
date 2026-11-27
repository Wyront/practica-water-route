using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed partial class CourierReviewsForm : BaseEditForm
{
    public CourierReviewsForm() : base("Мои отзывы", "Оценки курьера и заказов от клиентов", 1000, 700)
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        var grid = new DataGridView();
        UITheme.StyleGrid(grid);
        grid.Columns.Add("Order", "Заказ");
        grid.Columns.Add("Client", "Клиент");
        grid.Columns.Add("CourierRating", "Оценка курьера");
        grid.Columns.Add("CourierText", "Отзыв о курьере");
        grid.Columns.Add("OrderRating", "Оценка заказа");
        grid.Columns.Add("OrderText", "Отзыв о заказе");
        grid.Rows.Add("WR-10240", "Анна Воронова", "5", "Всё отлично", "4", "Быстрая доставка");
        grid.Rows.Add("WR-10239", "ООО «Ручеёк»", "5", "Вежливый курьер", "5", "Вода качественная");
        grid.Rows.Add("WR-10238", "Кафе «Север»", "4", "Без комментариев", "5", "Стабильно по утрам");
        Content.Controls.Add(grid);

        var stats = new Panel { Dock = DockStyle.Bottom, Height = 80, BackColor = UITheme.Surface, Padding = new Padding(14) };
        var title = UITheme.CreateLabel("Ваш рейтинг", 12F, FontStyle.Bold);
        title.Dock = DockStyle.Top;
        stats.Controls.Add(title);
        var rating = UITheme.CreateLabel("Средняя оценка: 4.8 ★  ·  Всего отзывов: 16  ·  Полных доставок: 42", 11F, FontStyle.Bold, UITheme.Primary);
        rating.Dock = DockStyle.Top;
        stats.Controls.Add(rating);
        Content.Controls.Add(stats);
    }
}