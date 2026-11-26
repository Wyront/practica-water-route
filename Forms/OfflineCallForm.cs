namespace WaterRoute.Forms;

public partial class OfflineCallForm : Form
{
    public OfflineCallForm() => InitializeComponent();

    private void copyButton_Click(object? sender, EventArgs e)
    {
        try
        {
            Clipboard.SetText("+79000000002");
            MessageBox.Show("Номер скопирован.", "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось скопировать номер: {ex.Message}", "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
