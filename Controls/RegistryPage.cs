namespace WaterRoute.Controls;

public sealed class RegistryOptions
{
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public required string[] Columns { get; init; }
    public required IReadOnlyList<string[]> Rows { get; init; }
    public string SearchPlaceholder { get; init; } = "Поиск";
    public string FilterLabel { get; init; } = "Статус:";
    public bool CanCreate { get; init; }
    public bool CanEdit { get; init; }
    public bool CanView { get; init; } = true;
    public bool CanDelete { get; init; }
    public string CreateText { get; init; } = "Создать";
    public string EditText { get; init; } = "Изменить";
    public string ViewText { get; init; } = "Просмотр";
    public string DeleteText { get; init; } = "Деактивировать";
    public string? AuxiliaryText { get; init; }
}

public sealed class RowEventArgs : EventArgs
{
    public RowEventArgs(string[] row) => Row = row;
    public string[] Row { get; }
}

public sealed partial class RegistryPage : BasePage
{
    private readonly List<string[]> _filteredRows = [];
    private RegistryOptions? _options;
    private int _currentPage = 1;
    private const int PageSize = 20;

    public event EventHandler? CreateRequested;
    public event EventHandler<RowEventArgs>? EditRequested;
    public event EventHandler<RowEventArgs>? ViewRequested;
    public event EventHandler<RowEventArgs>? DeleteRequested;
    public event EventHandler<RowEventArgs>? AuxiliaryRequested;
    public event EventHandler? RefreshRequested;

    public RegistryPage()
    {
        InitializeComponent();
    }

    public RegistryPage(RegistryOptions options) : base(options.Title, options.Subtitle)
    {
        InitializeComponent();
        SetOptions(options);
    }

    public string[]? SelectedRow => grid.CurrentRow?.Tag as string[];

    public void SetOptions(RegistryOptions options)
    {
        _options = options;
        titleLabel.Text = options.Title;
        subtitleLabel.Text = options.Subtitle;
        searchTextBox.PlaceholderText = options.SearchPlaceholder;
        filterCaptionLabel.Text = options.FilterLabel;
        createButton.Visible = options.CanCreate;
        createButton.Text = options.CreateText;
        editButton.Visible = options.CanEdit;
        editButton.Text = options.EditText;
        viewButton.Visible = options.CanView;
        viewButton.Text = options.ViewText;
        deleteButton.Visible = options.CanDelete;
        deleteButton.Text = options.DeleteText;
        auxiliaryButton.Visible = !string.IsNullOrWhiteSpace(options.AuxiliaryText);
        auxiliaryButton.Text = options.AuxiliaryText ?? string.Empty;
        grid.Columns.Clear();
        foreach (var column in options.Columns)
        {
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = column,
                SortMode = DataGridViewColumnSortMode.Automatic
            });
        }
        LoadFilter();
        ApplyFilters();
    }

    public void RefreshData()
    {
        RefreshRequested?.Invoke(this, EventArgs.Empty);
        ApplyFilters();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.F5)
        {
            RefreshData();
            return true;
        }
        if (keyData == (Keys.Control | Keys.F))
        {
            searchTextBox.Focus();
            searchTextBox.SelectAll();
            return true;
        }
        if (keyData == (Keys.Control | Keys.N) && _options?.CanCreate == true)
        {
            CreateRequested?.Invoke(this, EventArgs.Empty);
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void LoadFilter()
    {
        if (_options == null) return;
        filterComboBox.Items.Clear();
        filterComboBox.Items.Add("Все");
        var values = _options.Rows
            .Select(row => row.Length > 0 ? row[^1] : string.Empty)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value);
        foreach (var value in values)
            filterComboBox.Items.Add(value);
        filterComboBox.SelectedIndex = 0;
    }

    private void ApplyFilters()
    {
        if (_options == null) return;
        var search = searchTextBox.Text.Trim();
        var filter = filterComboBox.SelectedItem?.ToString() ?? "Все";
        var rows = _options.Rows.Where(row =>
            (search.Length == 0 || row.Any(value => value.Contains(search, StringComparison.OrdinalIgnoreCase))) &&
            (filter == "Все" || (row.Length > 0 && row[^1].Equals(filter, StringComparison.OrdinalIgnoreCase))));
        _filteredRows.Clear();
        _filteredRows.AddRange(rows);
        _currentPage = 1;
        RenderPage();
    }

    private void ChangePage(int page)
    {
        var pages = Math.Max(1, (int)Math.Ceiling(_filteredRows.Count / (double)PageSize));
        _currentPage = Math.Clamp(page, 1, pages);
        RenderPage();
    }

    private void RenderPage()
    {
        grid.Rows.Clear();
        foreach (var row in _filteredRows.Skip((_currentPage - 1) * PageSize).Take(PageSize))
        {
            var index = grid.Rows.Add(row);
            grid.Rows[index].Tag = row;
        }
        var pages = Math.Max(1, (int)Math.Ceiling(_filteredRows.Count / (double)PageSize));
        pageLabel.Text = $"Страница {_currentPage} из {pages}";
        countLabel.Text = $"Найдено: {_filteredRows.Count}";
        previousButton.Enabled = _currentPage > 1;
        nextButton.Enabled = _currentPage < pages;
    }

    private void WithSelected(Action<string[]> action)
    {
        if (SelectedRow is { } row)
            action(row);
        else
            MessageBox.Show("Сначала выберите строку в таблице.", "WaterRoute",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void createButton_Click(object? sender, EventArgs e) => CreateRequested?.Invoke(this, EventArgs.Empty);
    private void editButton_Click(object? sender, EventArgs e) => WithSelected(row => EditRequested?.Invoke(this, new RowEventArgs(row)));
    private void viewButton_Click(object? sender, EventArgs e) => WithSelected(row => ViewRequested?.Invoke(this, new RowEventArgs(row)));
    private void deleteButton_Click(object? sender, EventArgs e) => WithSelected(row => DeleteRequested?.Invoke(this, new RowEventArgs(row)));
    private void auxiliaryButton_Click(object? sender, EventArgs e) => WithSelected(row => AuxiliaryRequested?.Invoke(this, new RowEventArgs(row)));
    private void refreshButton_Click(object? sender, EventArgs e) => RefreshData();
    private void searchTextBox_TextChanged(object? sender, EventArgs e) => ApplyFilters();
    private void filterComboBox_SelectedIndexChanged(object? sender, EventArgs e) => ApplyFilters();
    private void previousButton_Click(object? sender, EventArgs e) => ChangePage(_currentPage - 1);
    private void nextButton_Click(object? sender, EventArgs e) => ChangePage(_currentPage + 1);

    private void grid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (_options?.CanEdit == true)
            WithSelected(row => EditRequested?.Invoke(this, new RowEventArgs(row)));
        else if (_options?.CanView == true)
            WithSelected(row => ViewRequested?.Invoke(this, new RowEventArgs(row)));
    }

    public void FocusSearch()
    {
        searchTextBox.Focus();
        searchTextBox.SelectAll();
    }

    public void CreateNew()
    {
        if (_options?.CanCreate == true)
            CreateRequested?.Invoke(this, EventArgs.Empty);
    }
}
