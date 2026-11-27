using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

public sealed partial class StorekeeperContainersForm : BaseEditForm
{
    public StorekeeperContainersForm() : base("Возвратная тара", "Подтверждение приёмки тары, долги клиентов и оборот", 1100, 700)
    {
        InitializeComponent();
    }

    private void RefreshData()
    {
        // TODO: sp_container_pending_list, sp_container_debt_get
    }

    private void confirmButton_Click(object? sender, EventArgs e)
    {
        if (containersGrid.SelectedRows.Count == 0) return;
        var row = (string[])containersGrid.SelectedRows[0].Tag!;
        using var form = new ContainerConfirmForm(row);
        if (form.ShowDialog(this) == DialogResult.OK)
            RefreshData();
    }

    private void debtButton_Click(object? sender, EventArgs e)
    {
        using var form = new ContainerDebtForm();
        form.ShowDialog(this);
    }

    private void reportButton_Click(object? sender, EventArgs e)
    {
        // TODO: sp_report_container_turnover
        MessageBox.Show("Отчёт по обороту тары будет сгенерирован после интеграции с БД", "WaterRoute");
    }

    private readonly INotificationService _notifications;
}