namespace WaterRoute.Infrastructure;

public static class UITheme
{
    public static readonly Color Canvas = Color.FromArgb(246, 248, 251);
    public static readonly Color Surface = Color.White;
    public static readonly Color SurfaceMuted = Color.FromArgb(241, 245, 249);
    public static readonly Color Text = Color.FromArgb(30, 41, 59);
    public static readonly Color TextMuted = Color.FromArgb(100, 116, 139);
    public static readonly Color Border = Color.FromArgb(218, 226, 235);
    public static readonly Color Primary = Color.FromArgb(37, 99, 235);
    public static readonly Color PrimaryDark = Color.FromArgb(29, 78, 216);
    public static readonly Color PrimarySoft = Color.FromArgb(231, 239, 253);
    public static readonly Color Success = Color.FromArgb(22, 163, 74);
    public static readonly Color Warning = Color.FromArgb(217, 119, 6);
    public static readonly Color Danger = Color.FromArgb(220, 38, 38);
    public static readonly Color Navigation = Color.FromArgb(24, 39, 65);
    public static readonly Color NavigationActive = Color.FromArgb(43, 64, 99);

    public static Font TitleFont(float size = 22F, FontStyle style = FontStyle.Bold) =>
        new("Segoe UI", size, style, GraphicsUnit.Point);

    public static Font BodyFont(float size = 10F, FontStyle style = FontStyle.Regular) =>
        new("Segoe UI", size, style, GraphicsUnit.Point);

    public static Label CreateLabel(string text, float size = 10F, FontStyle style = FontStyle.Regular,
        Color? color = null, bool autoSize = true) => new()
    {
        Text = text,
        Font = BodyFont(size, style),
        ForeColor = color ?? Text,
        AutoSize = autoSize,
        BackColor = Color.Transparent,
        Margin = new Padding(0, 4, 0, 4)
    };

    public static Button CreateButton(string text, bool primary = false, int width = 120)
    {
        var button = new Button
        {
            Text = text,
            Width = width,
            Height = 38,
            Font = BodyFont(9.5F, primary ? FontStyle.Bold : FontStyle.Regular),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            BackColor = primary ? Primary : Surface,
            ForeColor = primary ? Color.White : Text,
            Margin = new Padding(0, 0, 8, 8),
            UseVisualStyleBackColor = false
        };
        button.FlatAppearance.BorderColor = primary ? Primary : Border;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.MouseOverBackColor = primary ? PrimaryDark : PrimarySoft;
        return button;
    }

    public static TextBox CreateTextBox(string placeholder = "", bool multiline = false) => new()
    {
        PlaceholderText = placeholder,
        Font = BodyFont(),
        BorderStyle = BorderStyle.FixedSingle,
        BackColor = Surface,
        ForeColor = Text,
        Multiline = multiline,
        Margin = new Padding(0, 0, 8, 8)
    };

    public static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor = Surface;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.GridColor = Border;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        grid.ColumnHeadersDefaultCellStyle.BackColor = SurfaceMuted;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Text;
        grid.ColumnHeadersDefaultCellStyle.Font = BodyFont(9F, FontStyle.Bold);
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(6);
        grid.ColumnHeadersHeight = 42;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        grid.DefaultCellStyle.BackColor = Surface;
        grid.DefaultCellStyle.ForeColor = Text;
        grid.DefaultCellStyle.Font = BodyFont(9.5F);
        grid.DefaultCellStyle.SelectionBackColor = PrimarySoft;
        grid.DefaultCellStyle.SelectionForeColor = Text;
        grid.DefaultCellStyle.Padding = new Padding(5);
        grid.DefaultCellStyle.NullValue = "—";
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 255);
        grid.RowHeadersVisible = false;
        grid.RowTemplate.Height = 36;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.Dock = DockStyle.Fill;
    }

    public static Panel CreateCard(Color? backColor = null) => new()
    {
        BackColor = backColor ?? Surface,
        Padding = new Padding(18),
        Margin = new Padding(10),
        BorderStyle = BorderStyle.FixedSingle
    };
}
