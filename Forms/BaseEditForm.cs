namespace WaterRoute.Forms;

public partial class BaseEditForm : Form
{
    protected Panel Content => contentPanel;
    protected Button SaveButton => saveButton;

    public BaseEditForm()
    {
        InitializeComponent();
    }

    public BaseEditForm(string title, string subtitle, int width = 780, int height = 650) : this()
    {
        Text = title;
        titleLabel.Text = title;
        subtitleLabel.Text = subtitle;
        ClientSize = new Size(width, height);
        MinimumSize = new Size(Math.Min(width, 680), Math.Min(height, 560));
        subtitleLabel.MaximumSize = new Size(width - 60, 0);
    }

    protected virtual bool ValidateAndSave()
    {
        MessageBox.Show("Макет формы готов. Сохранение будет подключено после реализации процедур БД.",
            "Прототип WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return false;
    }

    protected void SaveButton_Click(object? sender, EventArgs e)
    {
        if (ValidateAndSave())
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    protected void SetFormHeader(string title, string subtitle)
    {
        Text = title;
        titleLabel.Text = title;
        subtitleLabel.Text = subtitle;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            DialogResult = DialogResult.Cancel;
            Close();
            e.Handled = true;
        }
        base.OnKeyDown(e);
    }
}
