using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

public sealed partial class StorekeeperStockForm : BaseEditForm
{
    public StorekeeperStockForm() : base("Складские остатки", "Физический остаток, резерв и доступное количество по складам", 1200, 700)
    {
        InitializeComponent();
    }

    private void RefreshData()
    {
        // TODO: sp_stock_list
    }

    private void lowStockButton_Click(object? sender, EventArgs e)
    {
        // TODO: sp_stock_low_list
        _notifications.ShowSuccess("Склад", "Показаны товары с низким остатком");
    }

    private void movementButton_Click(object? sender, EventArgs e)
    {
        using var form = new StockMovementForm();
        if (form.ShowDialog(this) == DialogResult.OK)
            RefreshData();
    }

    private void receiptButton_Click(object? sender, EventArgs e)
    {
        using var form = new StockReceiptForm();
        if (form.ShowDialog(this) == DialogResult.OK)
            RefreshData();
    }

    private void adjustmentButton_Click(object? sender, EventArgs e)
    {
        using var form = new StockAdjustmentForm();
        if (form.ShowDialog(this) == DialogResult.OK)
            RefreshData();
    }

    private void journalButton_Click(object? sender, EventArgs e)
    {
        using var form = new StockJournalForm();
        form.ShowDialog(this);
    }

    private readonly INotificationService _notifications;
}