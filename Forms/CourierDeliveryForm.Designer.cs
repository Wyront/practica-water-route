using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class CourierDeliveryForm
{
    private DataGridView itemsGrid = null!;
    private Button arrivedButton = null!;
    private Button completeButton = null!;
    private Button failButton = null!;
    private Button updateDelivered = null!;
    private Button returnContainer = null!;

    private void InitializeComponent()
    {
        var info = new Panel { Dock = DockStyle.Top, Height = 86, BackColor = UITheme.SurfaceMuted, Padding = new Padding(14) };
        var route = UITheme.CreateLabel("R-021  ·  Клиент №1  ·  +7 900 000-00-11", 11F, FontStyle.Bold);
        route.Dock = DockStyle.Top;
        info.Controls.Add(route);
        var address = UITheme.CreateLabel("Санкт-Петербург, Невский проспект, 10, кв. 5  ·  Окно 12:00–15:00", 9.5F,
            FontStyle.Regular, UITheme.TextMuted);
        address.Dock = DockStyle.Top;
        info.Controls.Add(address);
        Content.Controls.Add(info);

        var actions = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, FlowDirection = FlowDirection.LeftToRight };
        var start = UITheme.CreateButton("Начать маршрут", false, 145);
        start.Click += (_, _) => _notifications.ShowSuccess("Маршрут", "Маршрут начат");
        actions.Controls.Add(start);
        arrivedButton = UITheme.CreateButton("Курьер прибыл", false, 145);
        arrivedButton.Click += arrivedButton_Click;
        actions.Controls.Add(arrivedButton);
        var chat = UITheme.CreateButton("Чат с диспетчером", false, 175);
        chat.Click += (_, _) => MessageBox.Show("Чат откроется после интеграции", "WaterRoute");
        actions.Controls.Add(chat);
        Content.Controls.Add(actions);

        itemsGrid = new DataGridView();
        UITheme.StyleGrid(itemsGrid);
        itemsGrid.Columns.Add("Product", "Товар");
        itemsGrid.Columns.Add("Ordered", "Заказано");
        itemsGrid.Columns.Add("Delivered", "Передано");
        itemsGrid.Columns.Add("Returnable", "Возвратная тара");
        itemsGrid.Rows.Add("Вода питьевая 19 л", "4", "0", "Да");
        itemsGrid.Rows.Add("Вода питьевая 5 л", "6", "0", "Нет");
        itemsGrid.Rows.Add("Помпа механическая", "1", "0", "Нет");
        itemsGrid.CellDoubleClick += (_, _) => updateDelivered_Click(this, EventArgs.Empty);
        Content.Controls.Add(itemsGrid);

        var returnPanel = new Panel { Dock = DockStyle.Bottom, Height = 195, BackColor = UITheme.Surface, Padding = new Padding(14) };
        var returnTitle = UITheme.CreateLabel("Возврат тары и результат", 11F, FontStyle.Bold);
        returnTitle.Dock = DockStyle.Top;
        returnPanel.Controls.Add(returnTitle);
        var returnTable = FormLayout.CreateTwoColumnLayout();
        FormLayout.AddRow(returnTable, "Принято бутылей", FormLayout.CreateNumeric(0, 0, 10000));
        FormLayout.AddRow(returnTable, "Комментарий", UITheme.CreateTextBox("Комментарий к доставке или причина недоставки", true), 70);
        returnPanel.Controls.Add(returnTable);
        var resultButtons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 50, FlowDirection = FlowDirection.RightToLeft };
        failButton = UITheme.CreateButton("Не доставлено + проблема", false, 215);
        failButton.ForeColor = UITheme.Danger;
        failButton.Click += failButton_Click;
        resultButtons.Controls.Add(failButton);
        completeButton = UITheme.CreateButton("Доставлено полностью", true, 195);
        completeButton.Click += completeButton_Click;
        resultButtons.Controls.Add(completeButton);
        returnPanel.Controls.Add(resultButtons);
        Content.Controls.Add(returnPanel);

        var updateBtns = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 50, FlowDirection = FlowDirection.LeftToRight };
        updateDelivered = UITheme.CreateButton("Изменить передано", false, 150);
        updateDelivered.Click += updateDelivered_Click;
        updateBtns.Controls.Add(updateDelivered);
        returnContainer = UITheme.CreateButton("Возврат тары", false, 140);
        returnContainer.Click += returnContainer_Click;
        updateBtns.Controls.Add(returnContainer);
        Content.Controls.Add(updateBtns);

        Content.Controls.SetChildIndex(info, 0);
        Content.Controls.SetChildIndex(actions, 1);
        Content.Controls.SetChildIndex(returnPanel, 2);
        Content.Controls.SetChildIndex(itemsGrid, 3);
        Content.Controls.SetChildIndex(updateBtns, 4);
    }
}