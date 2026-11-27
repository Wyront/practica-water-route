using WaterRoute.Forms;
using WaterRoute.Infrastructure;

namespace WaterRoute.Controls;

partial class CatalogPage
{
    private Panel filtersPanel = null!;
    private ComboBox addressComboBox = null!;
    private TextBox searchTextBox = null!;
    private TableLayoutPanel tablesLayout = null!;
    private Panel productsPanel = null!;
    private DataGridView productsGrid = null!;
    private Button addButton = null!;
    private Panel cartPanel = null!;
    private DataGridView cartGrid = null!;
    private Panel cartBottomPanel = null!;
    private Label totalLabel = null!;
    private Button removeButton = null!;
    private Button checkoutButton = null!;

    private void InitializeComponent()
    {
        filtersPanel = new Panel { Dock = DockStyle.Top, Height = 86, BackColor = UITheme.Canvas };
        addressComboBox = FormLayout.CreateComboBox("Невский проспект, 10 · Центральный склад", "Приморский проспект, 25 · Северный склад");
        searchTextBox = UITheme.CreateTextBox("Поиск по названию или артикулу");
        tablesLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, BackColor = UITheme.Canvas };
        productsPanel = UITheme.CreateCard();
        productsGrid = new DataGridView();
        addButton = UITheme.CreateButton("Добавить в корзину", true, 180);
        cartPanel = UITheme.CreateCard();
        cartGrid = new DataGridView();
        cartBottomPanel = new Panel();
        totalLabel = UITheme.CreateLabel("Итого: 0,00 ₽", 14F, FontStyle.Bold, UITheme.Primary);
        removeButton = UITheme.CreateButton("Убрать", false, 95);
        checkoutButton = UITheme.CreateButton("Оформить", true, 115);
        tablesLayout.SuspendLayout();
        productsPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)productsGrid).BeginInit();
        cartPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)cartGrid).BeginInit();
        cartBottomPanel.SuspendLayout();
        SuspendLayout();
        // filtersPanel
        var addressLabel = UITheme.CreateLabel("Адрес доставки", 9F, FontStyle.Regular, UITheme.TextMuted);
        addressLabel.Location = new Point(0, 2);
        addressComboBox.SetBounds(0, 28, 520, 34);
        searchTextBox.SetBounds(540, 28, 340, 34);
        searchTextBox.TextChanged += searchTextBox_TextChanged;
        filtersPanel.Controls.Add(addressLabel);
        filtersPanel.Controls.Add(addressComboBox);
        filtersPanel.Controls.Add(searchTextBox);
        // tablesLayout
        tablesLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 61));
        tablesLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 39));
        tablesLayout.Controls.Add(productsPanel, 0, 0);
        tablesLayout.Controls.Add(cartPanel, 1, 0);
        // productsPanel
        productsPanel.Dock = DockStyle.Fill;
        var productsTitle = UITheme.CreateLabel("Доступные товары", 12F, FontStyle.Bold);
        productsTitle.Dock = DockStyle.Top;
        productsTitle.Height = 38;
        UITheme.StyleGrid(productsGrid);
        productsGrid.Columns.Add("Sku", "Артикул");
        productsGrid.Columns.Add("Name", "Товар");
        productsGrid.Columns.Add("Available", "Доступно");
        productsGrid.Columns.Add("Price", "Цена");
        productsGrid.Columns.Add("Returnable", "Тара");
        productsGrid.CellDoubleClick += productsGrid_CellDoubleClick;
        addButton.Dock = DockStyle.Bottom;
        addButton.Click += addButton_Click;
        productsPanel.Controls.Add(productsGrid);
        productsPanel.Controls.Add(addButton);
        productsPanel.Controls.Add(productsTitle);
        // cartPanel
        cartPanel.Dock = DockStyle.Fill;
        var cartTitle = UITheme.CreateLabel("Корзина", 12F, FontStyle.Bold);
        cartTitle.Dock = DockStyle.Top;
        cartTitle.Height = 38;
        UITheme.StyleGrid(cartGrid);
        cartGrid.Columns.Add("Name", "Товар");
        cartGrid.Columns.Add("Quantity", "Количество");
        cartGrid.Columns.Add("Price", "Цена");
        cartGrid.Columns.Add("Total", "Сумма");
        cartBottomPanel.Dock = DockStyle.Bottom;
        cartBottomPanel.Height = 108;
        cartBottomPanel.BackColor = UITheme.Surface;
        totalLabel.Dock = DockStyle.Top;
        totalLabel.Height = 34;
        removeButton.Location = new Point(0, 44);
        removeButton.Click += removeButton_Click;
        checkoutButton.Location = new Point(105, 44);
        checkoutButton.Click += checkoutButton_Click;
        cartBottomPanel.Controls.Add(totalLabel);
        cartBottomPanel.Controls.Add(removeButton);
        cartBottomPanel.Controls.Add(checkoutButton);
        cartPanel.Controls.Add(cartGrid);
        cartPanel.Controls.Add(cartBottomPanel);
        cartPanel.Controls.Add(cartTitle);
        // CatalogPage
        Content.Controls.Add(tablesLayout);
        Content.Controls.Add(filtersPanel);
        Name = "CatalogPage";
        tablesLayout.ResumeLayout(false);
        productsPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)productsGrid).EndInit();
        cartPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)cartGrid).EndInit();
        cartBottomPanel.ResumeLayout(false);
        ResumeLayout(false);
    }
}
