namespace WaterRoute.Forms;

public partial class DeliveryForm : BaseEditForm
{
    public DeliveryForm()
        : base("Обработка доставки · WR-10241", "Полная передача всех позиций либо недоставка без переданных товаров", 1050, 820)
    {
        InitializeComponent();
    }

    private void startButton_Click(object? sender, EventArgs e) => MessageBox.Show(
        "Начало маршрута будет зафиксировано после подключения процедур БД.",
        "Начало маршрута", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void arrivedButton_Click(object? sender, EventArgs e) => MessageBox.Show(
        "Прибытие курьера будет зафиксировано после подключения процедур БД.",
        "Курьер прибыл", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void chatButton_Click(object? sender, EventArgs e) => MessageBox.Show(
        "Чат с диспетчером будет доступен после подключения серверной части.",
        "Чат с диспетчером", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void failedButton_Click(object? sender, EventArgs e) => MessageBox.Show(
        "Регистрация недоставки и проблемы будет реализована после подключения процедур БД.",
        "Недоставка", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void deliveredButton_Click(object? sender, EventArgs e) => MessageBox.Show(
        "Подтверждение доставки будет реализовано после подключения процедур БД.",
        "Доставка", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
