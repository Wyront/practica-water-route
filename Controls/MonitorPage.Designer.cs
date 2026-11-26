using WaterRoute.Infrastructure;

namespace WaterRoute.Controls;

partial class MonitorPage
{
    private FlowLayoutPanel toolbarPanel = null!;
    private Button refreshButton = null!;
    private ComboBox statusComboBox = null!;
    private Label updatedLabel = null!;
    private TableLayoutPanel tablesPanel = null!;
    private DataGridView routesGrid = null!;
    private DataGridViewTextBoxColumn routeColumn = null!;
    private DataGridViewTextBoxColumn orderColumn = null!;
    private DataGridViewTextBoxColumn courierColumn = null!;
    private DataGridViewTextBoxColumn vehicleColumn = null!;
    private DataGridViewTextBoxColumn progressColumn = null!;
    private DataGridViewTextBoxColumn eventColumn = null!;
    private GroupBox eventsGroupBox = null!;
    private ListBox eventsListBox = null!;
    private FlowLayoutPanel actionsPanel = null!;
    private Button openRouteButton = null!;
    private Button chatButton = null!;
    private Button problemButton = null!;

    private void InitializeComponent()
    {
        toolbarPanel = new FlowLayoutPanel();
        refreshButton = new Button();
        statusComboBox = new ComboBox();
        updatedLabel = new Label();
        tablesPanel = new TableLayoutPanel();
        routesGrid = new DataGridView();
        routeColumn = new DataGridViewTextBoxColumn();
        orderColumn = new DataGridViewTextBoxColumn();
        courierColumn = new DataGridViewTextBoxColumn();
        vehicleColumn = new DataGridViewTextBoxColumn();
        progressColumn = new DataGridViewTextBoxColumn();
        eventColumn = new DataGridViewTextBoxColumn();
        eventsGroupBox = new GroupBox();
        eventsListBox = new ListBox();
        actionsPanel = new FlowLayoutPanel();
        openRouteButton = new Button();
        chatButton = new Button();
        problemButton = new Button();
        toolbarPanel.SuspendLayout();
        tablesPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)routesGrid).BeginInit();
        eventsGroupBox.SuspendLayout();
        actionsPanel.SuspendLayout();
        SuspendLayout();
        // 
        // toolbarPanel
        // 
        toolbarPanel.Controls.Add(refreshButton);
        toolbarPanel.Controls.Add(statusComboBox);
        toolbarPanel.Controls.Add(updatedLabel);
        toolbarPanel.Dock = DockStyle.Top;
        toolbarPanel.Height = 50;
        toolbarPanel.Name = "toolbarPanel";
        toolbarPanel.WrapContents = false;
        // 
        // refreshButton
        // 
        refreshButton.BackColor = UITheme.Primary;
        refreshButton.FlatStyle = FlatStyle.Flat;
        refreshButton.ForeColor = Color.White;
        refreshButton.Height = 38;
        refreshButton.Name = "refreshButton";
        refreshButton.Text = "Обновить  F5";
        refreshButton.Width = 125;
        refreshButton.Click += refreshButton_Click;
        // 
        // statusComboBox
        // 
        statusComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        statusComboBox.Items.AddRange(new object[] { "Все статусы", "Запланирован", "В работе", "Завершён" });
        statusComboBox.Name = "statusComboBox";
        statusComboBox.SelectedIndex = 0;
        statusComboBox.Width = 170;
        // 
        // updatedLabel
        // 
        updatedLabel.AutoSize = true;
        updatedLabel.ForeColor = UITheme.TextMuted;
        updatedLabel.Margin = new Padding(12, 8, 0, 0);
        updatedLabel.Name = "updatedLabel";
        updatedLabel.Text = $"Обновлено: {DateTime.Now:HH:mm:ss}";
        // 
        // tablesPanel
        // 
        tablesPanel.Controls.Add(routesGrid, 0, 0);
        tablesPanel.Controls.Add(eventsGroupBox, 0, 1);
        tablesPanel.Dock = DockStyle.Fill;
        tablesPanel.Name = "tablesPanel";
        tablesPanel.RowCount = 2;
        tablesPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 66F));
        tablesPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 34F));
        // 
        // routesGrid
        // 
        UITheme.StyleGrid(routesGrid);
        routesGrid.Columns.AddRange(new DataGridViewColumn[]
        {
            routeColumn,
            orderColumn,
            courierColumn,
            vehicleColumn,
            progressColumn,
            eventColumn
        });
        routesGrid.Name = "routesGrid";
        routesGrid.Rows.Add("R-021", "WR-10241", "Курьер Алексей", "А001АА78", "Курьер прибыл", "09:45");
        routesGrid.Rows.Add("R-022", "WR-10242", "Курьер Сергей", "А002АА78", "Запланирован", "09:20");
        routesGrid.CellDoubleClick += openRouteButton_Click;
        // 
        // routeColumn
        // 
        routeColumn.HeaderText = "Маршрут";
        routeColumn.Name = "Route";
        // 
        // orderColumn
        // 
        orderColumn.HeaderText = "Заказ";
        orderColumn.Name = "Order";
        // 
        // courierColumn
        // 
        courierColumn.HeaderText = "Курьер";
        courierColumn.Name = "Courier";
        // 
        // vehicleColumn
        // 
        vehicleColumn.HeaderText = "Автомобиль";
        vehicleColumn.Name = "Vehicle";
        // 
        // progressColumn
        // 
        progressColumn.HeaderText = "Статус";
        progressColumn.Name = "Progress";
        // 
        // eventColumn
        // 
        eventColumn.HeaderText = "Последнее событие";
        eventColumn.Name = "Event";
        // 
        // eventsGroupBox
        // 
        eventsGroupBox.Controls.Add(eventsListBox);
        eventsGroupBox.Controls.Add(actionsPanel);
        eventsGroupBox.Dock = DockStyle.Fill;
        eventsGroupBox.Name = "eventsGroupBox";
        eventsGroupBox.Padding = new Padding(12);
        eventsGroupBox.Text = "Последние события и проблемы";
        // 
        // eventsListBox
        // 
        eventsListBox.BorderStyle = BorderStyle.None;
        eventsListBox.Dock = DockStyle.Fill;
        eventsListBox.Items.AddRange(new object[]
        {
            "09:45 · R-021 · курьер прибыл по адресу",
            "09:35 · WR-10236 · создана проблема «Клиент не отвечает»",
            "09:20 · WR-10241 · маршрут назначен диспетчеру Анне",
            "09:00 · Центральный склад · низкий остаток воды 19 л"
        });
        eventsListBox.Name = "eventsListBox";
        // 
        // actionsPanel
        // 
        actionsPanel.Controls.Add(openRouteButton);
        actionsPanel.Controls.Add(chatButton);
        actionsPanel.Controls.Add(problemButton);
        actionsPanel.Dock = DockStyle.Bottom;
        actionsPanel.Height = 44;
        actionsPanel.Name = "actionsPanel";
        // 
        // openRouteButton
        // 
        openRouteButton.Name = "openRouteButton";
        openRouteButton.Text = "Открыть маршрут";
        openRouteButton.Width = 165;
        openRouteButton.Click += openRouteButton_Click;
        // 
        // chatButton
        // 
        chatButton.Name = "chatButton";
        chatButton.Text = "Открыть чат";
        chatButton.Width = 145;
        // 
        // problemButton
        // 
        problemButton.Name = "problemButton";
        problemButton.Text = "Зафиксировать проблему";
        problemButton.Width = 200;
        problemButton.Click += problemButton_Click;
        // 
        // MonitorPage
        // 
        Content.Controls.Add(tablesPanel);
        Content.Controls.Add(toolbarPanel);
        Name = "MonitorPage";
        Size = new Size(1000, 700);
        toolbarPanel.ResumeLayout(false);
        toolbarPanel.PerformLayout();
        tablesPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)routesGrid).EndInit();
        eventsGroupBox.ResumeLayout(false);
        actionsPanel.ResumeLayout(false);
        ResumeLayout(false);
    }
}
