using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed class ContainerDebtForm : BaseEditForm
{
    public ContainerDebtForm() : base("Долги клиентов по таре", "Количество выданных, возвращённых и текущий долг по типам тары", 900, 600)
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        var grid = new DataGridView();
        UITheme.StyleGrid(grid);
        grid.Columns.Add("Client", "Клиент");
        grid.Columns.Add("Type", "Тип тары");
        grid.Columns.Add("Issued", "Выдано");
        grid.Columns.Add("Returned", "Возвращено");
        grid.Columns.Add("Pending", "Ожидает");
        grid.Columns.Add("Debt", "Долг");
        grid.Rows.Add("Клиент №1", "Бутыль 19 л", "12", "8", "0", "4");
        grid.Rows.Add("Анна Воронова", "Бутыль 19 л", "6", "4", "2", "2");
        grid.Rows.Add("Кафе «Север»", "Бутыль 19 л", "8", "8", "0", "0");
        grid.Rows.Add("Клиент №1", "ПЭТ 5 л", "20", "20", "0", "0");
        Content.Controls.Add(grid);

        var export = UITheme.CreateButton("Экспорт долгов в CSV", false, 180);
        export.Dock = DockStyle.Bottom;
        export.Click += (_, _) => MessageBox.Show("Экспорт будет подключен после интеграции с БД", "WaterRoute");
        Content.Controls.Add(export);
    }
}