namespace WaterRoute.Controls;

public partial class BasePage : UserControl
{
    protected Panel Content => contentPanel;

    public BasePage()
    {
        InitializeComponent();
    }

    public BasePage(string title, string subtitle) : this()
    {
        titleLabel.Text = title;
        subtitleLabel.Text = subtitle;
    }
}
