using WaterRoute.Models;

namespace WaterRoute.Forms;

public partial class OrderEditForm : BaseEditForm
{
    public OrderEditForm() : this(UserRole.Dispatcher) { }

    public OrderEditForm(UserRole role, bool readOnly = false)
        : base("Новый заказ", "Один заказ обслуживается одним складом. Остатки проверяются перед созданием резерва.", 1040, 800)
    {
        InitializeComponent();
        SaveButton.Text = role == UserRole.Client ? "Оформить заказ" : "Сохранить заказ";
        SaveButton.Visible = !readOnly;
        addItemButton.Enabled = !readOnly;
        removeItemButton.Enabled = !readOnly;
    }

    private void addItemButton_Click(object? sender, EventArgs e) => MessageBox.Show(
        "Добавление товара будет реализовано после подключения каталога и процедур БД.",
        "Добавление товара", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void removeItemButton_Click(object? sender, EventArgs e) => MessageBox.Show(
        "Удаление позиции будет реализовано после подключения процедур БД.",
        "Удаление позиции", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void checkStockButton_Click(object? sender, EventArgs e) => MessageBox.Show(
        "После подключения процедур здесь будет выполнена проверка физического остатка и резерва.",
        "Проверка остатков", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
