using WaterRoute.Forms;
using WaterRoute.Models;

namespace WaterRoute.Controls;

public partial class ProfilePage : BasePage
{
    public ProfilePage() : base(
        "Профиль и адреса",
        "Личные данные, адресная книга и количественный долг по возвратной таре")
    {
        InitializeComponent();
    }

    public ProfilePage(UserSession session) : this()
    {
        nameLabel.Text = session.FullName;
        emailLabel.Text = $"{session.Email}  ·  +7 900 000-00-11";
    }

    private void addressButton_Click(object? sender, EventArgs e)
    {
        using var form = new AddressEditForm();
        form.ShowDialog(this);
    }
}
