using WaterRoute.Infrastructure;

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

    /// <summary>Синхронная проверка и сохранение. Для форм без обращения к БД.</summary>
    protected virtual bool ValidateAndSave()
    {
        MessageBox.Show("Эта форма пока работает только как макет: сохранение не подключено.",
            "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return false;
    }

    /// <summary>
    /// Асинхронная проверка и сохранение. Формы, работающие с БД, переопределяют именно этот метод:
    /// true — данные сохранены и окно можно закрывать, false — остаёмся в форме.
    /// </summary>
    protected virtual Task<bool> ValidateAndSaveAsync() => Task.FromResult(ValidateAndSave());

    protected async void SaveButton_Click(object? sender, EventArgs e)
    {
        saveButton.Enabled = false;
        try
        {
            if (await ValidateAndSaveAsync())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
        catch (Exception ex)
        {
            UiErrors.Show(ex, this);
        }
        finally
        {
            if (!IsDisposed)
                saveButton.Enabled = true;
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
