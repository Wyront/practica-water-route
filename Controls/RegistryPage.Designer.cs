#if NET10_0_OR_GREATER
#nullable enable
#endif

using WaterRoute.Infrastructure;

namespace WaterRoute.Controls;

partial class RegistryPage
{
    private FlowLayoutPanel actionsPanel = null!;
    private Button createButton = null!;
    private Button editButton = null!;
    private Button viewButton = null!;
    private Button deleteButton = null!;
    private Button auxiliaryButton = null!;
    private Button refreshButton = null!;
    private Panel filtersPanel = null!;
    private TextBox searchTextBox = null!;
    private Label filterCaptionLabel = null!;
    private ComboBox filterComboBox = null!;
    private Label countLabel = null!;
    private DataGridView grid = null!;
    private Panel pagerPanel = null!;
    private Button previousButton = null!;
    private Button nextButton = null!;
    private Label pageLabel = null!;

    private void InitializeComponent()
    {
        actionsPanel = new FlowLayoutPanel();
        createButton = UITheme.CreateButton("Создать", true);
        editButton = UITheme.CreateButton("Изменить");
        viewButton = UITheme.CreateButton("Просмотр");
        deleteButton = UITheme.CreateButton("Деактивировать");
        auxiliaryButton = UITheme.CreateButton("Действие", false, 190);
        refreshButton = UITheme.CreateButton("Обновить  F5", false, 125);
        filtersPanel = new Panel();
        searchTextBox = UITheme.CreateTextBox("Поиск");
        filterCaptionLabel = UITheme.CreateLabel("Статус:", 9.5F, FontStyle.Regular, UITheme.TextMuted);
        filterComboBox = new ComboBox();
        countLabel = UITheme.CreateLabel("Найдено: 0", 9F, FontStyle.Regular, UITheme.TextMuted, false);
        grid = new DataGridView();
        pagerPanel = new Panel();
        previousButton = UITheme.CreateButton("← Назад", false, 100);
        nextButton = UITheme.CreateButton("Вперёд →", false, 105);
        pageLabel = UITheme.CreateLabel("Страница 1 из 1", 9F, FontStyle.Regular, UITheme.TextMuted);
        actionsPanel.SuspendLayout();
        filtersPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
        pagerPanel.SuspendLayout();
        SuspendLayout();
        // actionsPanel
        actionsPanel.BackColor = UITheme.Canvas;
        actionsPanel.Controls.Add(createButton);
        actionsPanel.Controls.Add(editButton);
        actionsPanel.Controls.Add(viewButton);
        actionsPanel.Controls.Add(deleteButton);
        actionsPanel.Controls.Add(auxiliaryButton);
        actionsPanel.Controls.Add(refreshButton);
        actionsPanel.Dock = DockStyle.Top;
        actionsPanel.Height = 52;
        actionsPanel.WrapContents = false;
        // buttons
        createButton.Click += createButton_Click;
        editButton.Click += editButton_Click;
        viewButton.Click += viewButton_Click;
        deleteButton.ForeColor = UITheme.Danger;
        deleteButton.Click += deleteButton_Click;
        auxiliaryButton.Click += auxiliaryButton_Click;
        refreshButton.Click += refreshButton_Click;
        // filtersPanel
        filtersPanel.BackColor = UITheme.Canvas;
        filtersPanel.Controls.Add(searchTextBox);
        filtersPanel.Controls.Add(filterCaptionLabel);
        filtersPanel.Controls.Add(filterComboBox);
        filtersPanel.Controls.Add(countLabel);
        filtersPanel.Dock = DockStyle.Top;
        filtersPanel.Height = 52;
        filtersPanel.Resize += filtersPanel_Resize;
        // searchTextBox
        searchTextBox.SetBounds(0, 3, 360, 36);
        searchTextBox.TextChanged += searchTextBox_TextChanged;
        // filterCaptionLabel
        filterCaptionLabel.SetBounds(376, 10, 90, 28);
        // filterComboBox
        filterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        filterComboBox.Font = UITheme.BodyFont();
        filterComboBox.SetBounds(468, 5, 200, 34);
        filterComboBox.SelectedIndexChanged += filterComboBox_SelectedIndexChanged;
        // countLabel
        countLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        countLabel.SetBounds(700, 7, 300, 30);
        countLabel.TextAlign = ContentAlignment.MiddleRight;
        // grid
        UITheme.StyleGrid(grid);
        grid.CellDoubleClick += grid_CellDoubleClick;
        // pagerPanel
        pagerPanel.BackColor = UITheme.Canvas;
        pagerPanel.Controls.Add(previousButton);
        pagerPanel.Controls.Add(nextButton);
        pagerPanel.Controls.Add(pageLabel);
        pagerPanel.Dock = DockStyle.Bottom;
        pagerPanel.Height = 46;
        // pager controls
        previousButton.Location = new Point(0, 4);
        previousButton.Click += previousButton_Click;
        nextButton.Location = new Point(110, 4);
        nextButton.Click += nextButton_Click;
        pageLabel.Location = new Point(228, 12);
        // RegistryPage
        Content.Controls.Add(grid);
        Content.Controls.Add(pagerPanel);
        Content.Controls.Add(filtersPanel);
        Content.Controls.Add(actionsPanel);
        Name = "RegistryPage";
        Size = new Size(1000, 650);
        actionsPanel.ResumeLayout(false);
        filtersPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grid).EndInit();
        pagerPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    private void filtersPanel_Resize(object? sender, EventArgs e)
    {
        countLabel.Left = Math.Max(690, filtersPanel.ClientSize.Width - 310);
    }
}
