using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed class StockJournalForm : BaseEditForm
{
    public StockJournalForm() : base("Журнал складских движений", "История поступлений, списаний и корректировок", 1100, 700)
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        var grid = new DataGridView();
        UITheme.StyleGrid(grid);
        grid.Columns.Add("Date", "Дата и время");
        grid.Columns.Add("Warehouse", "Склад");
        grid.Columns.Add("Product", "Товар");
        grid.Columns.Add("Kind", "Тип");
        grid.Columns.Add("Qty", "Количество");
        grid.Columns.Add("Comment", "Комментарий");
        grid.Rows.Add("23.11.2026 10:15", "Центральный", "Вода 19 л", "Поступление", "+20", "Поставка от 23.11 №125");
        grid.Rows.Add("23.11.2026 09:02", "Центральный", "Вода 5 л", "Списание", "-12", "Доставка R-021");
        grid.Rows.Add("22.11.2026 16:40", "Северный", "Помпа", "Корректировка", "+5", "Инвентаризация");
        Content.Controls.Add(grid);
    }
}