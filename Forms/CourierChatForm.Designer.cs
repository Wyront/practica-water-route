#if NET10_0_OR_GREATER
#nullable enable
#endif

using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class CourierChatForm
{
    private System.ComponentModel.IContainer? components = null;

    private SplitContainer splitContainer = null!;
    private ListBox routesListBox = null!;
    private Label routeTitleLabel = null!;
    private Label participantsLabel = null!;
    private RichTextBox messagesRichTextBox = null!;
    private Panel inputPanel = null!;
    private TextBox inputTextBox = null!;
    private Button sendButton = null!;

    private void InitializeComponent()
    {
        splitContainer = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 250, FixedPanel = FixedPanel.Panel1 };
        var routesPanel = UITheme.CreateCard();
        routesPanel.Dock = DockStyle.Fill;
        var routesTitle = UITheme.CreateLabel("Мои маршруты", 12F, FontStyle.Bold);
        routesTitle.Dock = DockStyle.Top;
        routesTitle.Height = 38;
        routesListBox = new ListBox { Dock = DockStyle.Fill, Font = UITheme.BodyFont(), BorderStyle = BorderStyle.None };
        routesListBox.SelectedIndexChanged += routesListBox_SelectedIndexChanged;
        routesPanel.Controls.Add(routesListBox);
        routesPanel.Controls.Add(routesTitle);
        splitContainer.Panel1.Controls.Add(routesPanel);

        var chatPanel = new Panel { Dock = DockStyle.Fill };
        routeTitleLabel = UITheme.CreateLabel("Выберите маршрут", 12F, FontStyle.Bold);
        routeTitleLabel.Dock = DockStyle.Top;
        routeTitleLabel.Height = 34;
        participantsLabel = UITheme.CreateLabel("Курьер ↔ Диспетчер · история сохраняется", 9F, FontStyle.Regular, UITheme.TextMuted);
        participantsLabel.Dock = DockStyle.Top;
        participantsLabel.Height = 30;
        messagesRichTextBox = new RichTextBox
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            BorderStyle = BorderStyle.None,
            BackColor = UITheme.SurfaceMuted,
            Font = UITheme.BodyFont(),
            Padding = new Padding(12),
        };
        var inputPanel = new Panel { Dock = DockStyle.Bottom, Height = 96, BackColor = UITheme.Surface };
        inputTextBox = UITheme.CreateTextBox("Введите сообщение", true);
        inputTextBox.SetBounds(0, 8, 500, 64);
        inputTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        var sendButton = UITheme.CreateButton("Отправить", true, 125);
        sendButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        sendButton.SetBounds(540, 12, 125, 54);
        sendButton.Click += sendButton_Click;
        inputPanel.Controls.Add(inputTextBox);
        inputPanel.Controls.Add(sendButton);
        chatPanel.Controls.Add(messagesRichTextBox);
        chatPanel.Controls.Add(inputPanel);
        chatPanel.Controls.Add(participantsLabel);
        chatPanel.Controls.Add(routeTitleLabel);
        splitContainer.Panel2.Controls.Add(chatPanel);

        Content.Controls.Add(splitContainer);

        splitContainer.ResumeLayout(false);
        ResumeLayout(false);
    }
}