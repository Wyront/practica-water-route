using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

public sealed partial class ClientProfileForm : BaseEditForm
{
    public ClientProfileForm(UserSession session) : base("Профиль и адреса", "Личные данные, адресная книга и долг по таре", 900, 700)
    {
        _session = session;
        InitializeComponent();
        LoadProfile();
    }

    private readonly UserSession _session;

    private void LoadProfile()
    {
        nameTextBox.Text = _session.FullName;
        emailTextBox.Text = _session.Email;
        phoneTextBox.Text = "+7 900 000-00-11"; // TODO: из БД
        // TODO: загрузка адресов через sp_client_address_list
        // TODO: загрузка долга по таре через sp_container_debt_get
    }

    private void editProfileButton_Click(object? sender, EventArgs e)
    {
        nameTextBox.ReadOnly = false;
        phoneTextBox.ReadOnly = false;
        saveProfileButton.Visible = true;
        editProfileButton.Visible = false;
    }

    private async void saveProfileButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(nameTextBox.Text) || string.IsNullOrWhiteSpace(emailTextBox.Text))
        {
            MessageBox.Show("Заполните ФИО и e-mail", "WaterRoute");
            return;
        }
        // TODO: sp_user_change_password или обновление профиля
        MessageBox.Show("Профиль будет сохранён после интеграции с БД", "WaterRoute");
        nameTextBox.ReadOnly = true;
        phoneTextBox.ReadOnly = true;
        saveProfileButton.Visible = false;
        editProfileButton.Visible = true;
    }

    private void addAddressButton_Click(object? sender, EventArgs e)
    {
        using var form = new AddressEditForm();
        if (form.ShowDialog(this) == DialogResult.OK)
            LoadProfile(); // перезагрузка адресов
    }

    private void editAddressButton_Click(object? sender, EventArgs e)
    {
        if (addressesGrid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Выберите адрес", "WaterRoute");
            return;
        }
        var row = (string[])addressesGrid.SelectedRows[0].Tag!;
        using var form = new AddressEditForm(row);
        if (form.ShowDialog(this) == DialogResult.OK)
            LoadProfile();
    }

    private void setDefaultButton_Click(object? sender, EventArgs e)
    {
        if (addressesGrid.SelectedRows.Count == 0) return;
        var row = (string[])addressesGrid.SelectedRows[0].Tag!;
        var addressId = int.Parse(row[0]);
        // TODO: sp_client_address_set_default
        MessageBox.Show("Адрес по умолчанию будет установлен после интеграции с БД", "WaterRoute");
    }

    private void deactivateAddressButton_Click(object? sender, EventArgs e)
    {
        if (addressesGrid.SelectedRows.Count == 0) return;
        if (MessageBox.Show("Деактивировать адрес? Он останется в истории заказов.", "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            // TODO: деактивация адреса
            MessageBox.Show("Функция будет подключена после интеграции с БД", "WaterRoute");
        }
    }
}