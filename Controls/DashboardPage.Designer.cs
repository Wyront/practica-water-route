using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Controls;

partial class DashboardPage
{
    private TableLayoutPanel cardsLayout = null!;
    private Panel metricCard1, metricCard2, metricCard3, metricCard4, metricCard5;
    private Label metricTitle1, metricTitle2, metricTitle3, metricTitle4, metricTitle5;
    private Label metricValue1, metricValue2, metricValue3, metricValue4, metricValue5;
    private Label metricNote1, metricNote2, metricNote3, metricNote4, metricNote5;
    private TableLayoutPanel bottomLayout = null!;
    private Panel eventsPanel = null!;
    private DataGridView eventsGrid = null!;
    private Panel processPanel = null!;
    private Label processLabel = null!;

    private void InitializeComponent()
    {
        cardsLayout = new TableLayoutPanel { Dock = DockStyle.Top, Height = 166, ColumnCount = 5, BackColor = UITheme.Canvas, Padding = new Padding(0, 0, 0, 8) };
        bottomLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, BackColor = UITheme.Canvas };
        eventsPanel = UITheme.CreateCard();
        eventsGrid = new DataGridView();
        processPanel = UITheme.CreateCard();
        processLabel = UITheme.CreateLabel("", 9.5F, FontStyle.Regular, UITheme.Text, false);
        bottomLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)eventsGrid).BeginInit();
        SuspendLayout();
        for (var i = 0; i < 5; i++)
            cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
        CreateMetricCard(1, out metricCard1, out metricTitle1, out metricValue1, out metricNote1);
        CreateMetricCard(2, out metricCard2, out metricTitle2, out metricValue2, out metricNote2);
        CreateMetricCard(3, out metricCard3, out metricTitle3, out metricValue3, out metricNote3);
        CreateMetricCard(4, out metricCard4, out metricTitle4, out metricValue4, out metricNote4);
        CreateMetricCard(5, out metricCard5, out metricTitle5, out metricValue5, out metricNote5);
        cardsLayout.Controls.Add(metricCard1, 0, 0);
        cardsLayout.Controls.Add(metricCard2, 1, 0);
        cardsLayout.Controls.Add(metricCard3, 2, 0);
        cardsLayout.Controls.Add(metricCard4, 3, 0);
        cardsLayout.Controls.Add(metricCard5, 4, 0);
        // bottomLayout
        bottomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
        bottomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
        bottomLayout.Controls.Add(eventsPanel, 0, 0);
        bottomLayout.Controls.Add(processPanel, 1, 0);
        // eventsPanel
        eventsPanel.Dock = DockStyle.Fill;
        var eventsTitle = UITheme.CreateLabel("Последние события", 12F, FontStyle.Bold);
        eventsTitle.Dock = DockStyle.Top;
        eventsTitle.Height = 38;
        eventsPanel.Controls.Add(eventsGrid);
        eventsPanel.Controls.Add(eventsTitle);
        UITheme.StyleGrid(eventsGrid);
        eventsGrid.Columns.Add("Time", "Время");
        eventsGrid.Columns.Add("Event", "Событие");
        eventsGrid.Columns.Add("Status", "Статус");
        eventsGrid.Rows.Add("09:45", "R-021 · курьер прибыл по адресу", "В работе");
        eventsGrid.Rows.Add("09:20", "WR-10241 · товары зарезервированы", "Запланирован");
        eventsGrid.Rows.Add("08:55", "Вода 19 л · остаток ниже минимума", "Внимание");
        eventsGrid.Rows.Add("08:30", "Новое сообщение от диспетчера", "Новое");
        // processPanel
        processPanel.Dock = DockStyle.Fill;
        var processTitle = UITheme.CreateLabel("Текущий процесс", 12F, FontStyle.Bold);
        processTitle.Dock = DockStyle.Top;
        processTitle.Height = 38;
        processLabel.Dock = DockStyle.Fill;
        processLabel.Padding = new Padding(0, 8, 0, 0);
        processPanel.Controls.Add(processLabel);
        processPanel.Controls.Add(processTitle);
        // DashboardPage
        Content.Controls.Add(bottomLayout);
        Content.Controls.Add(cardsLayout);
        Name = "DashboardPage";
        bottomLayout.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)eventsGrid).EndInit();
        ResumeLayout(false);
    }

    private void CreateMetricCard(int index, out Panel card, out Label title, out Label value, out Label note)
    {
        card = UITheme.CreateCard();
        card.Dock = DockStyle.Fill;
        card.Cursor = Cursors.Hand;
        title = UITheme.CreateLabel($"Показатель {index}", 9F, FontStyle.Regular, UITheme.TextMuted);
        title.Dock = DockStyle.Top;
        value = UITheme.CreateLabel("0", 24F, FontStyle.Bold, UITheme.Primary);
        value.Dock = DockStyle.Top;
        value.Height = 56;
        note = UITheme.CreateLabel("Описание", 8.5F, FontStyle.Regular, UITheme.TextMuted);
        note.Dock = DockStyle.Top;
        card.Controls.Add(note);
        card.Controls.Add(value);
        card.Controls.Add(title);
        card.Click += metricCard_Click;
        title.Click += metricCard_Click;
        value.Click += metricCard_Click;
        note.Click += metricCard_Click;
    }
}
