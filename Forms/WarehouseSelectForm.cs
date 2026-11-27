using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed class WarehouseSelectForm : BaseEditForm
{
    private readonly IRepository _repo;
    public int? SelectedWarehouseId { get; private set; }
    private DataGridView grid = null!;

    public WarehouseSelectForm(IRepository repo)
        : base("Выбор склада", "Выберите склад для заказа", 600, 400)
    {
        _repo = repo;
        InitializeComponent();
        Load += (_, _) => LoadWarehouses();
    }

    private async void LoadWarehouses()
    {
        try
        {
            var rows = await _repo.QueryAsync<dynamic>("sp_directories_get_active", new { p_type = "warehouse" });
            
            grid.Rows.Clear();
            foreach (IDictionary<string, object?> r in rows)
            {
                var index = grid.Rows.Add(r["id"], r["name"], r["address"]);
                grid.Rows[index].Tag = r;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки складов: {ex.Message}", "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void InitializeComponent()
    {
        grid = new DataGridView();
        UITheme.StyleGrid(grid);
        grid.Columns.Add("Id", "ID");
        grid.Columns.Add("Name", "Склад");
        grid.Columns.Add("Address", "Адрес");
        grid.Columns["Id"].Visible = false;
        grid.DoubleClick += (_, _) => OkButton_Click(null, null);

        var btnOk = UITheme.CreateButton("Выбрать", true, 120);
        var btnCancel = UITheme.CreateButton("Отмена", false, 120);
        btnOk.Click += OkButton_Click;
        btnCancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 50,
            FlowDirection = FlowDirection.RightToLeft,
            BackColor = UITheme.Canvas
        };
        buttons.Controls.AddRange(new Control[] { btnCancel, btnOk });

        Content.Controls.Add(grid);
        Content.Controls.Add(buttons);
    }

    private void OkButton_Click(object? sender, EventArgs e)
    {
        if (grid.SelectedRows.Count == 0) return;
        var row = (IDictionary<string, object?>)grid.SelectedRows[0].Tag!;
        SelectedWarehouseId = Convert.ToInt32(row["id"]);
        DialogResult = DialogResult.OK;
        Close();
    }
}