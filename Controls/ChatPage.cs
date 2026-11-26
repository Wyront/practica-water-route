using WaterRoute.Models;

namespace WaterRoute.Controls;

public sealed partial class ChatPage : BasePage
{
    private readonly UserSession _session;

    public ChatPage() : this(RoleInfo.CreateDemoSession(UserRole.Dispatcher))
    {
    }

    public ChatPage(UserSession session) : base("Чаты по маршрутам",
        session.Role == UserRole.Admin ? "Административный просмотр истории без отправки сообщений" : "Сообщения курьера и назначенного диспетчера")
    {
        _session = session;
        InitializeComponent();
        sendButton.Enabled = session.Role != UserRole.Admin;
    }

    private void sendButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(inputTextBox.Text))
            return;
        messagesTextBox.AppendText($"\n\n[{DateTime.Now:HH:mm}] {_session.FullName}:\n{inputTextBox.Text.Trim()}");
        inputTextBox.Clear();
        messagesTextBox.SelectionStart = messagesTextBox.TextLength;
        messagesTextBox.ScrollToCaret();
    }
}
