using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

public sealed partial class CourierChatForm : BaseEditForm
{
    public CourierChatForm() : base("Чат с диспетчером · R-021",
        "Сообщения курьера и назначенного диспетчера", 800, 600)
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        var split = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 250, FixedPanel = FixedPanel.Panel1 };
        var routesPanel = UITheme.CreateCard();
        routesPanel.Dock = DockStyle.Fill;
        var routesTitle = UITheme.CreateLabel("Мои маршруты", 12F, FontStyle.Bold);
        routesTitle.Dock = DockStyle.Top;
        routesTitle.Height = 38;
        var routes = new ListBox { Dock = DockStyle.Fill, Font = UITheme.BodyFont(), BorderStyle = BorderStyle.None };
        routes.Items.AddRange(["R-021 · WR-10241 · активный", "R-020 · WR-10240 · завершён"]);
        routes.SelectedIndex = 0;
        routesPanel.Controls.Add(routes);
        routesPanel.Controls.Add(routesTitle);
        split.Panel1.Controls.Add(routesPanel);

        var chatPanel = UITheme.CreateCard();
        chatPanel.Dock = DockStyle.Fill;
        var routeTitle = UITheme.CreateLabel("R-021 · заказ WR-10241", 12F, FontStyle.Bold);
        routeTitle.Dock = DockStyle.Top;
        routeTitle.Height = 34;
        var participants = UITheme.CreateLabel("Курьер Алексей ↔ Диспетчер Анна · история сохраняется", 9F,
            FontStyle.Regular, UITheme.TextMuted);
        participants.Dock = DockStyle.Top;
        participants.Height = 30;
        var messages = new RichTextBox
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            BorderStyle = BorderStyle.None,
            BackColor = UITheme.SurfaceMuted,
            Font = UITheme.BodyFont(),
            Padding = new Padding(12),
            Text = "[09:12] Диспетчер Анна:\nДоброе утро! Подтвердите получение заказа на складе.\n\n[09:15] Курьер Алексей:\nЗаказ получил, всё соответствует. Выезжаю.\n\n[09:45] Курьер Алексей:\nПрибыл по адресу, клиент открывает дверь."
        };
        var inputPanel = new Panel { Dock = DockStyle.Bottom, Height = 96, BackColor = UITheme.Surface };
        var input = UITheme.CreateTextBox("Введите сообщение", true);
        input.SetBounds(0, 8, 500, 64);
        input.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        var send = UITheme.CreateButton("Отправить", true, 125);
        send.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        send.SetBounds(540, 12, 125, 54);
        send.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(input.Text))
                return;
            messages.AppendText($"\n\n[{DateTime.Now:HH:mm}] Курьер Алексей:\n{input.Text.Trim()}");
            input.Clear();
            messages.SelectionStart = messages.TextLength;
            messages.ScrollToCaret();
        };
        inputPanel.Controls.Add(input);
        inputPanel.Controls.Add(send);
        chatPanel.Controls.Add(messages);
        chatPanel.Controls.Add(inputPanel);
        chatPanel.Controls.Add(participants);
        chatPanel.Controls.Add(routeTitle);
        split.Panel2.Controls.Add(chatPanel);
        Content.Controls.Add(split);
    }
}