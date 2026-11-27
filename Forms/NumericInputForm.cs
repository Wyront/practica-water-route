using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed class NumericInputForm : Form
{
    public int ResultValue { get; private set; }

    public NumericInputForm(string title, int maxValue)
    {
        Text = title;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(360, 160);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = UITheme.Canvas;
        Font = UITheme.BodyFont();

        var label = UITheme.CreateLabel($"Введите значение (макс. {maxValue}):", 10F, FontStyle.Regular);
        label.Location = new Point(20, 20);
        Controls.Add(label);

        var numeric = new NumericUpDown
        {
            Minimum = 0,
            Maximum = maxValue,
            Value = 0,
            Font = UITheme.BodyFont(14F),
            Width = 200,
            Location = new Point(20, 55)
        };
        Controls.Add(numeric);

        var ok = UITheme.CreateButton("OK", true, 100);
        ok.Location = new Point(80, 100);
        ok.Click += (_, _) =>
        {
            ResultValue = (int)numeric.Value;
            DialogResult = DialogResult.OK;
            Close();
        };
        Controls.Add(ok);

        var cancel = UITheme.CreateButton("Отмена", false, 100);
        cancel.Location = new Point(200, 100);
        cancel.DialogResult = DialogResult.Cancel;
        Controls.Add(cancel);

        AcceptButton = ok;
        CancelButton = cancel;
    }
}