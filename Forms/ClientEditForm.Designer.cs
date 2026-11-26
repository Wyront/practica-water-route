using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class ClientEditForm
{
    private TableLayoutPanel fields = null!;
    private TextBox fullNameTextBox = null!;
    private TextBox emailTextBox = null!;
    private TextBox phoneTextBox = null!;
    private CheckBox activeCheckBox = null!;
    private FlowLayoutPanel addressToolbar = null!;
    private Button addAddressButton = null!;
    private Button editAddressButton = null!;
    private Button defaultAddressButton = null!;
    private DataGridView addressesGrid = null!;

    private void InitializeComponent()
    {
        fields = FormLayout.CreateTwoColumnLayout();
        fullNameTextBox = UITheme.CreateTextBox();
        emailTextBox = UITheme.CreateTextBox();
        phoneTextBox = UITheme.CreateTextBox();
        activeCheckBox = new CheckBox();
        addressToolbar = new FlowLayoutPanel();
        addAddressButton = UITheme.CreateButton("Добавить адрес", true, 150);
        editAddressButton = UITheme.CreateButton("Изменить адрес", false, 150);
        defaultAddressButton = UITheme.CreateButton("Сделать основным", false, 155);
        addressesGrid = new DataGridView();
        fields.SuspendLayout();
        addressToolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)addressesGrid).BeginInit();
        SuspendLayout();
        //
        // fields
        //
        fullNameTextBox.Text = "Клиент №1";
        FormLayout.AddRow(fields, "ФИО *", fullNameTextBox, 52);
        emailTextBox.Text = "client1@water.test";
        FormLayout.AddRow(fields, "E-mail *", emailTextBox, 52);
        phoneTextBox.Text = "+7 900 000-00-11";
        FormLayout.AddRow(fields, "Телефон", phoneTextBox, 52);
        activeCheckBox.AutoSize = true;
        activeCheckBox.Checked = true;
        activeCheckBox.CheckState = CheckState.Checked;
        activeCheckBox.Name = "activeCheckBox";
        activeCheckBox.Text = "Клиент активен";
        activeCheckBox.UseVisualStyleBackColor = true;
        FormLayout.AddRow(fields, "Активность", activeCheckBox, 52);
        //
        // addressToolbar
        //
        addressToolbar.Controls.Add(addAddressButton);
        addressToolbar.Controls.Add(editAddressButton);
        addressToolbar.Controls.Add(defaultAddressButton);
        addressToolbar.Dock = DockStyle.Top;
        addressToolbar.Height = 50;
        addressToolbar.Name = "addressToolbar";
        addressToolbar.WrapContents = false;
        addAddressButton.Name = "addAddressButton";
        addAddressButton.Click += addressButton_Click;
        editAddressButton.Name = "editAddressButton";
        editAddressButton.Click += addressButton_Click;
        defaultAddressButton.Name = "defaultAddressButton";
        //
        // addressesGrid
        //
        UITheme.StyleGrid(addressesGrid);
        addressesGrid.Columns.Add("Address", "Адрес");
        addressesGrid.Columns.Add("Warehouse", "Склад");
        addressesGrid.Columns.Add("Default", "По умолчанию");
        addressesGrid.Columns.Add("Active", "Активен");
        addressesGrid.Name = "addressesGrid";
        addressesGrid.Rows.Add("Невский проспект, 10, кв. 5", "Центральный склад", "Да", "Да");
        addressesGrid.Rows.Add("Приморский проспект, 25, офис 301", "Северный склад", "Нет", "Да");
        //
        // ClientEditForm
        //
        Content.Controls.Add(addressesGrid);
        Content.Controls.Add(addressToolbar);
        Content.Controls.Add(fields);
        Name = "ClientEditForm";
        fields.ResumeLayout(false);
        fields.PerformLayout();
        addressToolbar.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)addressesGrid).EndInit();
        ResumeLayout(false);
    }
}
