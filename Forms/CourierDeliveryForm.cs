using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

public sealed partial class CourierDeliveryForm : BaseEditForm
{
    public CourierDeliveryForm() : base("Обработка доставки · WR-10241",
        "Принцип «всё или ничего»: полная передача всех позиций либо недоставка без переданных товаров", 1050, 820)
    {
        InitializeComponent();
    }

    private void arrivedButton_Click(object? sender, EventArgs e)
    {
        // TODO: sp_route_arrive
        _notifications.ShowSuccess("Доставка", "Прибытие отмечено. Статус точки: «Курьер прибыл»");
    }

    private void completeButton_Click(object? sender, EventArgs e)
    {
        // Проверяем, что все позиции переданы полностью
        foreach (DataGridViewRow r in itemsGrid.Rows)
        {
            if (r.Tag is string[] row)
            {
                var ordered = int.Parse(row[1]);
                var delivered = int.Parse(row[2]);
                if (delivered != ordered)
                {
                    MessageBox.Show("Для полной доставки все позиции должны быть переданы полностью. Используйте кнопку «Не доставлено» при несоответствии.", "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
        }

        // TODO: sp_delivery_complete с позициями и возвратом тары
        _notifications.ShowSuccess("Доставка", "Доставка завершена полностью. Товары списаны, тара выдана клиенту");
        DialogResult = DialogResult.OK;
        Close();
    }

    private void failButton_Click(object? sender, EventArgs e)
    {
        if (MessageBox.Show("Отметить как «Не доставлено»? Никакие товары не будут переданы, резерв сохранится, создастся проблема.", "Подтверждение недоставки",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
        {
            // TODO: sp_delivery_fail
            _notifications.ShowWarning("Доставка", "Маршрут отмечен как «Не доставлено». Проблема зарегистрирована автоматически");
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    private void updateDelivered_Click(object? sender, EventArgs e)
    {
        if (itemsGrid.SelectedRows.Count == 0) return;
        var row = itemsGrid.SelectedRows[0];
        var maxQty = int.Parse(row.Cells[1].Value?.ToString() ?? "0");
        using var form = new NumericInputForm("Количество передано", maxQty);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            row.Cells[2].Value = form.ResultValue;
            row.Cells[2].Style.ForeColor = form.ResultValue > 0 ? UITheme.Success : UITheme.Text;
        }
    }

    private void returnContainer_Click(object? sender, EventArgs e)
    {
        if (itemsGrid.SelectedRows.Count == 0) return;
        var row = itemsGrid.SelectedRows[0];
        if (!(bool)row.Cells[3].Value!)
        {
            MessageBox.Show("Этот товар не имеет возвратной тары", "WaterRoute");
            return;
        }
        var maxQty = int.Parse(row.Cells[1].Value?.ToString() ?? "0");
        using var form = new NumericInputForm("Количество принятых бутылей", maxQty);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            // TODO: sp_container_movement_return_by_courier
            _notifications.ShowSuccess("Тара", $"Принято бутылей: {form.ResultValue}. Ожидает подтверждения кладовщиком");
        }
    }

    private readonly INotificationService _notifications;
}