using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public static class FormLayout
{
    public static TableLayoutPanel CreateTwoColumnLayout()
    {
        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            BackColor = UITheme.Canvas,
            Padding = new Padding(0, 0, 0, 8)
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        return table;
    }

    public static void AddRow(TableLayoutPanel table, string caption, Control control, int height = 48)
    {
        var row = table.RowCount++;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
        var label = UITheme.CreateLabel(caption, 9.5F, FontStyle.Regular, UITheme.TextMuted);
        label.Anchor = AnchorStyles.Left;
        label.Margin = new Padding(0, 0, 12, 8);
        control.Dock = DockStyle.Fill;
        control.Margin = new Padding(0, 0, 0, 8);
        table.Controls.Add(label, 0, row);
        table.Controls.Add(control, 1, row);
    }

    public static ComboBox CreateComboBox(params string[] items)
    {
        var combo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = UITheme.BodyFont()
        };
        combo.Items.AddRange(items);
        if (combo.Items.Count > 0)
            combo.SelectedIndex = 0;
        return combo;
    }

    public static NumericUpDown CreateNumeric(decimal value, decimal minimum, decimal maximum, int decimalPlaces = 0)
    {
        return new NumericUpDown
        {
            Value = value,
            Minimum = minimum,
            Maximum = maximum,
            DecimalPlaces = decimalPlaces,
            ThousandsSeparator = true,
            Font = UITheme.BodyFont()
        };
    }

    public static Panel CreateGroup(string title, Control content, int height = 180)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Top,
            Height = height,
            BackColor = UITheme.Surface,
            Padding = new Padding(16),
            Margin = new Padding(0, 0, 0, 12)
        };
        var caption = UITheme.CreateLabel(title, 11F, FontStyle.Bold);
        caption.Dock = DockStyle.Top;
        caption.Height = 32;
        content.Dock = DockStyle.Fill;
        content.Margin = new Padding(0, 8, 0, 0);
        panel.Controls.Add(content);
        panel.Controls.Add(caption);
        return panel;
    }
}
