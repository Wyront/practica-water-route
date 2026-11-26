using WaterRoute.Infrastructure;

namespace WaterRoute.Controls;

partial class NotificationsPage
{
    private DataGridView _grid = null!;
    private Button markAllReadButton = null!;
    private FlowLayoutPanel actionsPanel = null!;
    private Panel filtersPanel = null!;
    private TextBox searchTextBox = null!;
    private Label filterCaptionLabel = null!;
    private ComboBox filterComboBox = null!;
    private Label countLabel = null!;
    private Panel pagerPanel = null!;
    private Button previousButton = null!;
    private Button nextButton = null!;
    private Label pageLabel = null!;

    private void InitializeComponent()
    {
        actionsPanel = new FlowLayoutPanel();
        markAllReadButton = UITheme.CreateButton("Отметить все прочитанными", false, 220);
        _grid = new DataGridView();
        filtersPanel = new Panel();
        searchTextBox = UITheme.CreateTextBox("Поиск по уведомлениям");
        filterCaptionLabel = UITheme.CreateLabel("Тип:", 9.5F, FontStyle.Regular, UITheme.TextMuted);
        filterComboBox = new ComboBox();
        countLabel = UITheme.CreateLabel("Найдено: 0", 9F, FontStyle.Regular, UITheme.TextMuted, false);
        pagerPanel = new Panel();
        previousButton = UITheme.CreateButton("← Назад", false, 100);
        nextButton = UITheme.CreateButton("Вперёд →", false, 105);
        pageLabel = UITheme.CreateLabel("Страница 1 из 1", 9F, FontStyle.Regular, UITheme.TextMuted);

        actionsPanel.SuspendLayout();
        filtersPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_grid).BeginInit();
        pagerPanel.SuspendLayout();
        SuspendLayout();

        // actionsPanel
        actionsPanel.BackColor = UITheme.Canvas;
        actionsPanel.Controls.Add(markAllReadButton);
        actionsPanel.Dock = DockStyle.Top;
        actionsPanel.Height = 52;
        actionsPanel.WrapContents = false;
        markAllReadButton.Click += markAllReadButton_Click;

        // filtersPanel
        filtersPanel.BackColor = UITheme.Canvas;
        filtersPanel.Controls.Add(searchTextBox);
        filtersPanel.Controls.Add(filterCaptionLabel);
        filtersPanel.Controls.Add(filterComboBox);
        filtersPanel.Controls.Add(countLabel);
        filtersPanel.Dock = DockStyle.Top;
        filtersPanel.Height = 52;
        filtersPanel.Resize += filtersPanel_Resize;
        searchTextBox.SetBounds(0, 3, 360, 36);
        filterCaptionLabel.SetBounds(376, 10, 90, 28);
        filterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        filterComboBox.Font = UITheme.BodyFont();
        filterComboBox.SetBounds(468, 5, 200, 34);
        filterComboBox.Items.AddRange(new object[] { "Все", "order_status", "stock_low", "chat", "container_pending" });
        filterComboBox.SelectedIndex = 0;
        countLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        countLabel.SetBounds(700, 7, 300, 30);
        countLabel.TextAlign = ContentAlignment.MiddleRight;

        // _grid
        UITheme.StyleGrid(_grid);
        _grid.Columns.Add("Id", "ID");
        _grid.Columns.Add("Time", "Время");
        _grid.Columns.Add("Type", "Тип");
        _grid.Columns.Add("Title", "Заголовок");
        _grid.Columns.Add("Message", "Сообщение");
        _grid.Columns.Add("Read", "Прочитано");
        _grid.CellDoubleClick += grid_CellDoubleClick;

        // pagerPanel
        pagerPanel.BackColor = UITheme.Canvas;
        pagerPanel.Controls.Add(previousButton);
        pagerPanel.Controls.Add(nextButton);
        pagerPanel.Controls.Add(pageLabel);
        pagerPanel.Dock = DockStyle.Bottom;
        pagerPanel.Height = 46;
        previousButton.Location = new Point(0, 4);
        nextButton.Location = new Point(110, 4);
        pageLabel.Location = new Point(228, 12);

        // NotificationsPage
        Content.Controls.Add(_grid);
        Content.Controls.Add(pagerPanel);
        Content.Controls.Add(filtersPanel);
        Content.Controls.Add(actionsPanel);
        Name = "NotificationsPage";
        Size = new Size(1000, 650);
        actionsPanel.ResumeLayout(false);
        filtersPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_grid).EndInit();
        pagerPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    private void filtersPanel_Resize(object? sender, EventArgs e)
    {
        countLabel.Left = Math.Max(690, filtersPanel.ClientSize.Width - 310);
    }
}