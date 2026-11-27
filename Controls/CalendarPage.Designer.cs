using WaterRoute.Infrastructure;

namespace WaterRoute.Controls;

partial class CalendarPage
{
    private FlowLayoutPanel toolbarPanel = null!;
    private ComboBox modeComboBox = null!;
    private Button previousButton = null!;
    private Button todayButton = null!;
    private Button nextButton = null!;
    private Label periodLabel = null!;
    private SplitContainer splitContainer = null!;
    private MonthCalendar monthCalendar = null!;
    private DataGridView eventsGrid = null!;
    private DataGridViewTextBoxColumn timeColumn = null!;
    private DataGridViewTextBoxColumn orderColumn = null!;
    private DataGridViewTextBoxColumn clientColumn = null!;
    private DataGridViewTextBoxColumn routeColumn = null!;
    private DataGridViewTextBoxColumn statusColumn = null!;

    private void InitializeComponent()
    {
        toolbarPanel = new FlowLayoutPanel();
        modeComboBox = new ComboBox();
        previousButton = new Button();
        todayButton = new Button();
        nextButton = new Button();
        periodLabel = new Label();
        splitContainer = new SplitContainer();
        monthCalendar = new MonthCalendar();
        eventsGrid = new DataGridView();
        timeColumn = new DataGridViewTextBoxColumn();
        orderColumn = new DataGridViewTextBoxColumn();
        clientColumn = new DataGridViewTextBoxColumn();
        routeColumn = new DataGridViewTextBoxColumn();
        statusColumn = new DataGridViewTextBoxColumn();
        toolbarPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)eventsGrid).BeginInit();
        SuspendLayout();
        // 
        // toolbarPanel
        // 
        toolbarPanel.Controls.Add(modeComboBox);
        toolbarPanel.Controls.Add(previousButton);
        toolbarPanel.Controls.Add(todayButton);
        toolbarPanel.Controls.Add(nextButton);
        toolbarPanel.Controls.Add(periodLabel);
        toolbarPanel.Dock = DockStyle.Top;
        toolbarPanel.Height = 52;
        toolbarPanel.Name = "toolbarPanel";
        toolbarPanel.WrapContents = false;
        // 
        // modeComboBox
        // 
        modeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        modeComboBox.Items.AddRange(new object[] { "Месяц", "Неделя" });
        modeComboBox.Name = "modeComboBox";
        modeComboBox.SelectedIndex = 0;
        modeComboBox.Width = 130;
        // 
        // previousButton
        // 
        previousButton.Name = "previousButton";
        previousButton.Text = "←";
        previousButton.Width = 48;
        // 
        // todayButton
        // 
        todayButton.Name = "todayButton";
        todayButton.Text = "Сегодня";
        todayButton.Width = 105;
        // 
        // nextButton
        // 
        nextButton.Name = "nextButton";
        nextButton.Text = "→";
        nextButton.Width = 48;
        // 
        // periodLabel
        // 
        periodLabel.AutoSize = true;
        periodLabel.Font = UITheme.BodyFont(12F, FontStyle.Bold);
        periodLabel.Margin = new Padding(16, 8, 0, 0);
        periodLabel.Name = "periodLabel";
        periodLabel.Text = "Ноябрь 2026";
        // 
        // splitContainer
        // 
        splitContainer.Dock = DockStyle.Fill;
        splitContainer.FixedPanel = FixedPanel.Panel1;
        splitContainer.Name = "splitContainer";
        // 
        // splitContainer.Panel1
        // 
        splitContainer.Panel1.Controls.Add(monthCalendar);
        // 
        // splitContainer.Panel2
        // 
        splitContainer.Panel2.Controls.Add(eventsGrid);
        splitContainer.SplitterDistance = 315;
        // 
        // monthCalendar
        // 
        monthCalendar.Dock = DockStyle.Fill;
        monthCalendar.MaxSelectionCount = 1;
        monthCalendar.Name = "monthCalendar";
        monthCalendar.ShowTodayCircle = true;
        monthCalendar.TitleBackColor = UITheme.Primary;
        monthCalendar.TitleForeColor = Color.White;
        // 
        // eventsGrid
        // 
        UITheme.StyleGrid(eventsGrid);
        eventsGrid.Columns.AddRange(new DataGridViewColumn[]
        {
            timeColumn,
            orderColumn,
            clientColumn,
            routeColumn,
            statusColumn
        });
        eventsGrid.Name = "eventsGrid";
        eventsGrid.Rows.Add("09:00–12:00", "WR-10239", "ООО «Нева»", "R-019", "Доставлен");
        eventsGrid.Rows.Add("12:00–15:00", "WR-10241", "Клиент №1", "R-021", "Запланирован");
        eventsGrid.Rows.Add("15:00–18:00", "WR-10242", "Кафе «Север»", "R-022", "В доставке");
        eventsGrid.CellDoubleClick += eventsGrid_CellDoubleClick;
        // 
        // timeColumn
        // 
        timeColumn.HeaderText = "Время";
        timeColumn.Name = "Time";
        // 
        // orderColumn
        // 
        orderColumn.HeaderText = "Заказ";
        orderColumn.Name = "Order";
        // 
        // clientColumn
        // 
        clientColumn.HeaderText = "Клиент";
        clientColumn.Name = "Client";
        // 
        // routeColumn
        // 
        routeColumn.HeaderText = "Маршрут";
        routeColumn.Name = "Route";
        // 
        // statusColumn
        // 
        statusColumn.HeaderText = "Статус";
        statusColumn.Name = "Status";
        // 
        // CalendarPage
        // 
        Content.Controls.Add(splitContainer);
        Content.Controls.Add(toolbarPanel);
        Name = "CalendarPage";
        Size = new Size(1000, 700);
        toolbarPanel.ResumeLayout(false);
        toolbarPanel.PerformLayout();
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)eventsGrid).EndInit();
        ResumeLayout(false);
    }
}
