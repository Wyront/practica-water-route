using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

partial class MainForm
{
    private Panel sidePanel = null!;
    private Label logoLabel = null!;
    private Label cityLabel = null!;
    private FlowLayoutPanel navigationPanel = null!;
    private Panel topPanel = null!;
    private Label roleLabel = null!;
    private Label nameLabel = null!;
    private Button logoutButton = null!;
    private Button notificationsButton = null!;
    private Button offlineButton = null!;
    private Panel contentPanel = null!;

    private void InitializeComponent()
    {
        sidePanel = new Panel();
        logoLabel = UITheme.CreateLabel("WaterRoute", 21F, FontStyle.Bold, Color.White);
        cityLabel = UITheme.CreateLabel("Санкт-Петербург", 9F, FontStyle.Regular, Color.FromArgb(148, 163, 184));
        navigationPanel = new FlowLayoutPanel();
        topPanel = new Panel();
        roleLabel = UITheme.CreateLabel("Роль", 10F, FontStyle.Bold, UITheme.Primary);
        nameLabel = UITheme.CreateLabel("Пользователь", 9F, FontStyle.Regular, UITheme.TextMuted);
        logoutButton = UITheme.CreateButton("Выход", false, 90);
        notificationsButton = UITheme.CreateButton("Уведомления", false, 135);
        offlineButton = UITheme.CreateButton("Офлайн-режим", false, 160);
        contentPanel = new Panel();
        sidePanel.SuspendLayout();
        topPanel.SuspendLayout();
        SuspendLayout();
        // sidePanel
        sidePanel.BackColor = UITheme.Navigation;
        sidePanel.Controls.Add(navigationPanel);
        sidePanel.Controls.Add(logoLabel);
        sidePanel.Controls.Add(cityLabel);
        sidePanel.Dock = DockStyle.Left;
        sidePanel.Width = 228;
        // logoLabel / cityLabel
        logoLabel.Location = new Point(22, 22);
        cityLabel.Location = new Point(24, 62);
        // navigationPanel
        navigationPanel.AutoScroll = true;
        navigationPanel.BackColor = UITheme.Navigation;
        navigationPanel.Dock = DockStyle.Fill;
        navigationPanel.FlowDirection = FlowDirection.TopDown;
        navigationPanel.Padding = new Padding(12, 105, 12, 12);
        navigationPanel.WrapContents = false;
        AddNavigationButtons();
        // topPanel
        topPanel.BackColor = UITheme.Surface;
        topPanel.Controls.Add(roleLabel);
        topPanel.Controls.Add(nameLabel);
        topPanel.Controls.Add(logoutButton);
        topPanel.Controls.Add(notificationsButton);
        topPanel.Controls.Add(offlineButton);
        topPanel.Dock = DockStyle.Top;
        topPanel.Height = 70;
        // roleLabel / nameLabel
        roleLabel.Location = new Point(24, 16);
        nameLabel.Location = new Point(24, 39);
        // logoutButton
        logoutButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        logoutButton.Location = new Point(1102, 16);
        logoutButton.Click += logoutButton_Click;
        // notificationsButton
        notificationsButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        notificationsButton.Location = new Point(952, 16);
        notificationsButton.Click += notificationsButton_Click;
        // offlineButton
        offlineButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        offlineButton.Location = new Point(772, 16);
        offlineButton.Click += offlineButton_Click;
        // contentPanel
        contentPanel.BackColor = UITheme.Canvas;
        contentPanel.Dock = DockStyle.Fill;
        // MainForm
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UITheme.Canvas;
        ClientSize = new Size(1440, 900);
        Controls.Add(contentPanel);
        Controls.Add(topPanel);
        Controls.Add(sidePanel);
        Font = UITheme.BodyFont();
        KeyPreview = true;
        MinimumSize = new Size(1180, 720);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "WaterRoute";
        Shown += MainForm_Shown;
        sidePanel.ResumeLayout(false);
        sidePanel.PerformLayout();
        topPanel.ResumeLayout(false);
        topPanel.PerformLayout();
        ResumeLayout(false);
    }

    private void AddNavigationButtons()
    {
        navigationPanel.Controls.Add(CreateNavigationButton("Главная", PageId.Dashboard));
        navigationPanel.Controls.Add(CreateNavigationButton("Пользователи", PageId.Users));
        navigationPanel.Controls.Add(CreateNavigationButton("Справочники", PageId.Directories));
        navigationPanel.Controls.Add(CreateNavigationButton("Клиенты", PageId.Clients));
        navigationPanel.Controls.Add(CreateNavigationButton("Каталог и корзина", PageId.Catalog));
        navigationPanel.Controls.Add(CreateNavigationButton("Заказы", PageId.Orders));
        navigationPanel.Controls.Add(CreateNavigationButton("Маршруты", PageId.Routes));
        navigationPanel.Controls.Add(CreateNavigationButton("Склад", PageId.Stock));
        navigationPanel.Controls.Add(CreateNavigationButton("Возвратная тара", PageId.Containers));
        navigationPanel.Controls.Add(CreateNavigationButton("Проблемы", PageId.Problems));
        navigationPanel.Controls.Add(CreateNavigationButton("Отзывы", PageId.Reviews));
        navigationPanel.Controls.Add(CreateNavigationButton("Чаты", PageId.Chat));
        navigationPanel.Controls.Add(CreateNavigationButton("Отчёты", PageId.Reports));
        navigationPanel.Controls.Add(CreateNavigationButton("Календарь", PageId.Calendar));
        navigationPanel.Controls.Add(CreateNavigationButton("Монитор", PageId.Monitor));
        navigationPanel.Controls.Add(CreateNavigationButton("Импорт CSV", PageId.Import));
        navigationPanel.Controls.Add(CreateNavigationButton("Уведомления", PageId.Notifications));
        navigationPanel.Controls.Add(CreateNavigationButton("Карта", PageId.Map));
        navigationPanel.Controls.Add(CreateNavigationButton("Профиль и адреса", PageId.Profile));
    }

    private Button CreateNavigationButton(string text, PageId pageId)
    {
        var button = new Button
        {
            Text = text,
            Tag = pageId,
            Width = 196,
            Height = 44,
            Margin = new Padding(0, 0, 0, 4),
            Padding = new Padding(14, 0, 6, 0),
            TextAlign = ContentAlignment.MiddleLeft,
            FlatStyle = FlatStyle.Flat,
            BackColor = UITheme.Navigation,
            ForeColor = Color.FromArgb(226, 232, 240),
            Font = UITheme.BodyFont(10F),
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = UITheme.NavigationActive;
        button.Click += navigationButton_Click;
        return button;
    }
}
