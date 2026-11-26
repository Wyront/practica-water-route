namespace WaterRoute.Forms;

public partial class ClientEditForm : BaseEditForm
{
    public ClientEditForm()
        : base("Карточка клиента", "Контактные данные и адресная книга клиента", 900, 700)
    {
        InitializeComponent();
    }

    private void addressButton_Click(object? sender, EventArgs e)
    {
        using var form = new AddressEditForm();
        form.ShowDialog(this);
    }
}
