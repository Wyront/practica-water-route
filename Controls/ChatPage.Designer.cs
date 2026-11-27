using WaterRoute.Infrastructure;

namespace WaterRoute.Controls;

partial class ChatPage
{
    private SplitContainer splitContainer = null!;
    private ListBox routesListBox = null!;
    private RichTextBox messagesTextBox = null!;
    private TextBox inputTextBox = null!;
    private Button sendButton = null!;

    private void InitializeComponent()
    {
        splitContainer = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 285, FixedPanel = FixedPanel.Panel1, BackColor = UITheme.Canvas };
        routesListBox = new ListBox();
        messagesTextBox = new RichTextBox();
        inputTextBox = UITheme.CreateTextBox("Введите сообщение", true);
        sendButton = UITheme.CreateButton("Отправить", true, 125);
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        SuspendLayout();
        // Panel1
        var routesPanel = UITheme.CreateCard();
        routesPanel.Dock = DockStyle.Fill;
        var routesTitle = UITheme.CreateLabel("Маршруты", 12F, FontStyle.Bold);
        routesTitle.Dock = DockStyle.Top;
        routesTitle.Height = 38;
        routesListBox.Dock = DockStyle.Fill;
        routesListBox.Font = UITheme.BodyFont();
        routesListBox.BorderStyle = BorderStyle.None;
        routesListBox.Items.AddRange(["R-021 · WR-10241 · активный", "R-018 · WR-10235 · завершён", "R-012 · WR-10220 · завершён"]);
        routesListBox.SelectedIndex = 0;
        routesPanel.Controls.Add(routesListBox);
        routesPanel.Controls.Add(routesTitle);
        splitContainer.Panel1.Controls.Add(routesPanel);
        // Panel2
        var chatPanel = UITheme.CreateCard();
        chatPanel.Dock = DockStyle.Fill;
        var routeTitle = UITheme.CreateLabel("R-021 · заказ WR-10241", 12F, FontStyle.Bold);
        routeTitle.Dock = DockStyle.Top;
        routeTitle.Height = 34;
        var participants = UITheme.CreateLabel("Курьер Алексей ↔ Диспетчер Анна · история сохраняется", 9F, FontStyle.Regular, UITheme.TextMuted);
        participants.Dock = DockStyle.Top;
        participants.Height = 30;
        messagesTextBox.Dock = DockStyle.Fill;
        messagesTextBox.ReadOnly = true;
        messagesTextBox.BorderStyle = BorderStyle.None;
        messagesTextBox.BackColor = UITheme.SurfaceMuted;
        messagesTextBox.Font = UITheme.BodyFont();
        messagesTextBox.Text = "[09:12] Диспетчер Анна:\nДоброе утро! Подтвердите получение заказа на складе.\n\n[09:15] Курьер Алексей:\nЗаказ получил, всё соответствует. Выезжаю.\n\n[09:45] Курьер Алексей:\nПрибыл по адресу, клиент открывает дверь.";
        var inputPanel = new Panel { Dock = DockStyle.Bottom, Height = 96, BackColor = UITheme.Surface };
        inputTextBox.SetBounds(0, 8, 500, 64);
        inputTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sendButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        sendButton.SetBounds(540, 12, 125, 54);
        sendButton.Click += sendButton_Click;
        inputPanel.Controls.Add(inputTextBox);
        inputPanel.Controls.Add(sendButton);
        chatPanel.Controls.Add(messagesTextBox);
        chatPanel.Controls.Add(inputPanel);
        chatPanel.Controls.Add(participants);
        chatPanel.Controls.Add(routeTitle);
        splitContainer.Panel2.Controls.Add(chatPanel);
        // ChatPage
        Content.Controls.Add(splitContainer);
        Name = "ChatPage";
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
        ResumeLayout(false);
    }
}
