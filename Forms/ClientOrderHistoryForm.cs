using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

public sealed partial class ClientOrderHistoryForm : BaseEditForm
{
    public ClientOrderHistoryForm() : base("Мои заказы", "История заказов, статусы и повторные заказы", 1100, 700)
    {
        InitializeComponent();
    }

    private void RefreshData()
    {
        // TODO: sp_order_list с фильтром по client_id
    }

    private void repeatOrderButton_Click(object? sender, EventArgs e)
    {
        if (ordersGrid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Выберите заказ для повтора", "WaterRoute");
            return;
        }
        var row = (string[])ordersGrid.SelectedRows[0].Tag!;
        var orderId = int.Parse(row[0]);
        // TODO: sp_order_repeat
        MessageBox.Show("Повтор заказа будет выполнен после интеграции с БД", "WaterRoute");
    }

    private void cancelOrderButton_Click(object? sender, EventArgs e)
    {
        if (ordersGrid.SelectedRows.Count == 0) return;
        var row = (string[])ordersGrid.SelectedRows[0].Tag!;
        var status = row[5];
        if (status != "Новый")
        {
            MessageBox.Show("Отменить можно только заказ в статусе «Новый\". Для других статусов обратитесь к диспетчеру.", "WaterRoute");
            return;
        }
        if (MessageBox.Show("Отменить выбранный заказ?", "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            // TODO: sp_order_cancel
            MessageBox.Show("Заказ будет отменён после интеграции с БД", "WaterRoute");
        }
    }

    private void viewOrderButton_Click(object? sender, EventArgs e)
    {
        if (ordersGrid.SelectedRows.Count == 0) return;
        var row = (string[])ordersGrid.SelectedRows[0].Tag!;
        using var form = new OrderEditForm(UserRole.Client, true); // read-only
        form.ShowDialog(this);
    }

    private void pdfButton_Click(object? sender, EventArgs e)
    {
        if (ordersGrid.SelectedRows.Count == 0) return;
        // TODO: sp_pdf_order_card
        MessageBox.Show("PDF будет сгенерирован после интеграции с БД", "WaterRoute");
    }
}