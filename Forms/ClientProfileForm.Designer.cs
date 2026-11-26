using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class ClientProfileForm
{
    private TextBox nameTextBox = null!;
    private TextBox emailTextBox = null!;
    private TextBox phoneTextBox = null!;
    private Button editProfileButton = null!;
    private Button saveProfileButton = null!;
    private DataGridView addressesGrid = null!;
    private Button addAddressButton = null!;
    private Button editAddressButton = null!;
    private Button setDefaultButton = null!;
    private Button deactivateAddressButton = null!;
    private Label debtLabel = null!;

    private void InitializeComponent()
    {
        nameTextBox = UITheme.CreateTextBox("Иванов Иван Иванович");
        emailTextBox = UITheme.CreateTextBox("client1@water.test");
        phoneTextBox = UITheme.CreateTextBox("+7 900 000-00-11");
        editProfileButton = UITheme.CreateButton("Редактировать", false, 140);
        saveProfileButton = UITheme.CreateButton("Сохранить", true, 140);
        saveProfileButton.Visible = false;

        addressesGrid = new DataGridView();
        UITheme.StyleGrid(addressesGrid);
        addressesGrid.Columns.Add("Id", "ID");
        addressesGrid.Columns.Add("Address", "Адрес");
        addressesGrid.Columns.Add("Warehouse", "Склад");
        addressesGrid.Columns.Add("Coordinates", "Координаты");
        addressesGrid.Columns.Add("Default", "По умолчанию");
        addressesGrid.Columns.Add("Active", "Активен");
        addressesGrid.Rows.Add("1", "Невский проспект, 10, кв. 5", "Центральный склад", "59.9343, 30.3351", "Да", "Да");
        addressesGrid.Rows.Add("2", "Приморский проспект, 25, офис 301", "Северный склад", "60.0152, 30.3204", "Нет", "Да");
        foreach (DataGridViewRow r in addressesGrid.Rows) r.Tag = new[] { r.Cells[0].Value?.ToString()!, r.Cells[1].Value?.ToString()! };

        addAddressButton = UITheme.CreateButton("Добавить адрес", true, 150);
        editAddressButton = UITheme.CreateButton("Изменить", false, 130);
        setDefaultButton = UITheme.CreateButton("Сделать основным", false, 150);
        deactivateAddressButton = UITheme.CreateButton("Деактивировать", false, 140);
        deactivateAddressButton.ForeColor = UITheme.Danger;

        debtLabel = UITheme.CreateLabel("Долг по таре: 4 бутыли 19 л  ·  Ожидает подтверждения: 2", 10F, FontStyle.Bold, UITheme.Warning);

        var profilePanel = new Panel { Dock = DockStyle.Top, Height = 180, BackColor = UITheme.Surface, Padding = new Padding(14) };
        var table = FormLayout.CreateTwoColumnLayout();
        FormLayout.AddRow(table, "ФИО *", nameTextBox);
        FormLayout.AddRow(table, "E-mail *", emailTextBox);
        FormLayout.AddRow(table, "Телефон", phoneTextBox);
        profilePanel.Controls.Add(table);
        var profileButtons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 44, FlowDirection = FlowDirection.RightToLeft };
        editProfileButton.Click += editProfileButton_Click;
        saveProfileButton.Click += saveProfileButton_Click;
        profileButtons.Controls.Add(saveProfileButton);
        profileButtons.Controls.Add(editProfileButton);
        profilePanel.Controls.Add(profileButtons);

        var addressToolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, FlowDirection = FlowDirection.LeftToRight };
        addAddressButton.Click += addAddressButton_Click;
        editAddressButton.Click += editAddressButton_Click;
        setDefaultButton.Click += setDefaultButton_Click;
        deactivateAddressButton.Click += deactivateAddressButton_Click;
        addressToolbar.Controls.AddRange(new Control[] { addAddressButton, editAddressButton, setDefaultButton, deactivateAddressButton });

        var debtPanel = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = UITheme.Warning, Padding = new Padding(14) };
        debtLabel.Dock = DockStyle.Fill;
        debtPanel.Controls.Add(debtLabel);

        Content.Controls.Add(addressesGrid);
        Content.Controls.Add(addressToolbar);
        Content.Controls.Add(debtPanel);
        Content.Controls.Add(profilePanel);
    }
}