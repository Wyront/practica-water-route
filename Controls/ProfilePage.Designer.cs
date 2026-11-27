using WaterRoute.Infrastructure;

namespace WaterRoute.Controls;

partial class ProfilePage
{
    private Panel profilePanel = null!;
    private Label nameLabel = null!;
    private Label emailLabel = null!;
    private Button editProfileButton = null!;
    private Label debtLabel = null!;
    private FlowLayoutPanel toolbarPanel = null!;
    private Button addAddressButton = null!;
    private Button editAddressButton = null!;
    private Button defaultAddressButton = null!;
    private Button deactivateButton = null!;
    private DataGridView addressesGrid = null!;
    private DataGridViewTextBoxColumn addressColumn = null!;
    private DataGridViewTextBoxColumn warehouseColumn = null!;
    private DataGridViewTextBoxColumn coordinatesColumn = null!;
    private DataGridViewTextBoxColumn defaultColumn = null!;
    private DataGridViewTextBoxColumn activeColumn = null!;

    private void InitializeComponent()
    {
        profilePanel = new Panel();
        nameLabel = new Label();
        emailLabel = new Label();
        editProfileButton = new Button();
        debtLabel = new Label();
        toolbarPanel = new FlowLayoutPanel();
        addAddressButton = new Button();
        editAddressButton = new Button();
        defaultAddressButton = new Button();
        deactivateButton = new Button();
        addressesGrid = new DataGridView();
        addressColumn = new DataGridViewTextBoxColumn();
        warehouseColumn = new DataGridViewTextBoxColumn();
        coordinatesColumn = new DataGridViewTextBoxColumn();
        defaultColumn = new DataGridViewTextBoxColumn();
        activeColumn = new DataGridViewTextBoxColumn();
        profilePanel.SuspendLayout();
        toolbarPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)addressesGrid).BeginInit();
        SuspendLayout();
        // 
        // profilePanel
        // 
        profilePanel.BackColor = UITheme.Surface;
        profilePanel.Controls.Add(nameLabel);
        profilePanel.Controls.Add(emailLabel);
        profilePanel.Controls.Add(editProfileButton);
        profilePanel.Controls.Add(debtLabel);
        profilePanel.Dock = DockStyle.Top;
        profilePanel.Height = 116;
        profilePanel.Name = "profilePanel";
        // 
        // nameLabel
        // 
        nameLabel.AutoSize = true;
        nameLabel.Font = UITheme.BodyFont(13F, FontStyle.Bold);
        nameLabel.Location = new Point(14, 12);
        nameLabel.Name = "nameLabel";
        nameLabel.Text = "Клиент";
        // 
        // emailLabel
        // 
        emailLabel.AutoSize = true;
        emailLabel.ForeColor = UITheme.TextMuted;
        emailLabel.Location = new Point(14, 44);
        emailLabel.Name = "emailLabel";
        emailLabel.Text = "client@water.test";
        // 
        // editProfileButton
        // 
        editProfileButton.FlatStyle = FlatStyle.Flat;
        editProfileButton.Name = "editProfileButton";
        editProfileButton.SetBounds(14, 70, 190, 38);
        editProfileButton.Text = "Редактировать профиль";
        // 
        // debtLabel
        // 
        debtLabel.AutoSize = true;
        debtLabel.Font = UITheme.BodyFont(10F, FontStyle.Bold);
        debtLabel.ForeColor = UITheme.Warning;
        debtLabel.Location = new Point(330, 78);
        debtLabel.Name = "debtLabel";
        debtLabel.Text = "Долг по таре: 4  ·  Ожидает подтверждения: 2";
        // 
        // toolbarPanel
        // 
        toolbarPanel.Controls.Add(addAddressButton);
        toolbarPanel.Controls.Add(editAddressButton);
        toolbarPanel.Controls.Add(defaultAddressButton);
        toolbarPanel.Controls.Add(deactivateButton);
        toolbarPanel.Dock = DockStyle.Top;
        toolbarPanel.Height = 50;
        toolbarPanel.Name = "toolbarPanel";
        toolbarPanel.WrapContents = false;
        // 
        // addAddressButton
        // 
        addAddressButton.BackColor = UITheme.Primary;
        addAddressButton.FlatStyle = FlatStyle.Flat;
        addAddressButton.ForeColor = Color.White;
        addAddressButton.Height = 38;
        addAddressButton.Name = "addAddressButton";
        addAddressButton.Text = "Добавить адрес";
        addAddressButton.Width = 150;
        addAddressButton.Click += addressButton_Click;
        // 
        // editAddressButton
        // 
        editAddressButton.Name = "editAddressButton";
        editAddressButton.Text = "Изменить адрес";
        editAddressButton.Width = 155;
        editAddressButton.Click += addressButton_Click;
        // 
        // defaultAddressButton
        // 
        defaultAddressButton.Name = "defaultAddressButton";
        defaultAddressButton.Text = "Сделать основным";
        defaultAddressButton.Width = 155;
        // 
        // deactivateButton
        // 
        deactivateButton.Name = "deactivateButton";
        deactivateButton.Text = "Деактивировать";
        deactivateButton.Width = 145;
        // 
        // addressesGrid
        // 
        UITheme.StyleGrid(addressesGrid);
        addressesGrid.Columns.AddRange(new DataGridViewColumn[]
        {
            addressColumn,
            warehouseColumn,
            coordinatesColumn,
            defaultColumn,
            activeColumn
        });
        addressesGrid.Name = "addressesGrid";
        addressesGrid.Rows.Add("Невский проспект, 10, кв. 5", "Центральный склад", "59.934300, 30.335100", "Да", "Да");
        addressesGrid.Rows.Add("Приморский проспект, 25, офис 301", "Северный склад", "60.015200, 30.320400", "Нет", "Да");
        addressesGrid.CellDoubleClick += addressButton_Click;
        // 
        // addressColumn
        // 
        addressColumn.HeaderText = "Адрес";
        addressColumn.Name = "Address";
        // 
        // warehouseColumn
        // 
        warehouseColumn.HeaderText = "Склад";
        warehouseColumn.Name = "Warehouse";
        // 
        // coordinatesColumn
        // 
        coordinatesColumn.HeaderText = "Координаты";
        coordinatesColumn.Name = "Coordinates";
        // 
        // defaultColumn
        // 
        defaultColumn.HeaderText = "По умолчанию";
        defaultColumn.Name = "Default";
        // 
        // activeColumn
        // 
        activeColumn.HeaderText = "Активен";
        activeColumn.Name = "Active";
        // 
        // ProfilePage
        // 
        Content.Controls.Add(addressesGrid);
        Content.Controls.Add(toolbarPanel);
        Content.Controls.Add(profilePanel);
        Name = "ProfilePage";
        Size = new Size(1000, 700);
        profilePanel.ResumeLayout(false);
        profilePanel.PerformLayout();
        toolbarPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)addressesGrid).EndInit();
        ResumeLayout(false);
    }
}
