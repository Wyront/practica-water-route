using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

public sealed partial class DispatcherClientForm : BaseEditForm
{
    public DispatcherClientForm() : base("Работа с клиентами", "Создание клиентов, управление адресами и просмотр истории заказов", 1100, 700)
    {
        InitializeComponent();
    }

    private void RefreshData()
    {
        // TODO: sp_client_list
    }

    private void createButton_Click(object? sender, EventArgs e)
    {
        using var form = new RegistrationForm();
        if (form.ShowDialog(this) == DialogResult.OK)
            RefreshData();
    }

    private void editButton_Click(object? sender, EventArgs e)
    {
        if (clientsGrid.SelectedRows.Count == 0) return;
        var row = (string[])clientsGrid.SelectedRows[0].Tag!;
        using var form = new ClientEditForm();
        // TODO: передать row
        form.ShowDialog(this);
        RefreshData();
    }

    private void addressesButton_Click(object? sender, EventArgs e)
    {
        if (clientsGrid.SelectedRows.Count == 0) return;
        var row = (string[])clientsGrid.SelectedRows[0].Tag!;
        // TODO: открыть адреса клиента
        MessageBox.Show("Управление адресами клиента будет подключено после интеграции с БД", "WaterRoute");
    }

    private void historyButton_Click(object? sender, EventArgs e)
    {
        if (clientsGrid.SelectedRows.Count == 0) return;
        var row = (string[])clientsGrid.SelectedRows[0].Tag!;
        // TODO: открыть историю заказов клиента
        MessageBox.Show("История заказов клиента будет подключена после интеграции с БД", "WaterRoute");
    }

    private void callButton_Click(object? sender, EventArgs e)
    {
        if (clientsGrid.SelectedRows.Count == 0) return;
        var row = (string[])clientsGrid.SelectedRows[0].Tag!;
        var phone = row[2];
        if (MessageBox.Show($"Позвонить клиенту по номеру {phone}?", "Звонок",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            // TODO: интеграция с телефонией или копирование в буфер
            Clipboard.SetText(phone);
            _notifications.ShowSuccess("Звонок", $"Номер {phone} скопирован в буфер обмена");
        }
    }

    private readonly INotificationService _notifications;
}