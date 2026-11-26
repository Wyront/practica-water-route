using WaterRoute.Forms;
using WaterRoute.Models;

namespace WaterRoute.Controls;

public sealed partial class CatalogPage : BasePage
{
    private sealed record Product(string Name, string Sku, int Available, decimal Price, bool Returnable);

    private readonly Product[] _products =
    [
        new("Вода питьевая 19 л", "WT-019", 84, 250m, true),
        new("Вода питьевая 5 л", "WT-005", 126, 90m, false),
        new("Вода минеральная 19 л", "WM-019", 0, 300m, true),
        new("Помпа механическая", "AC-001", 12, 450m, false)
    ];

    public CatalogPage() : base("Каталог и корзина", "Остатки рассчитываются для склада выбранного адреса: физический остаток минус резерв")
    {
        InitializeComponent();
        LoadProducts(string.Empty);
    }

    private void LoadProducts(string search)
    {
        productsGrid.Rows.Clear();
        foreach (var product in _products.Where(p => search.Length == 0 ||
                     p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                     p.Sku.Contains(search, StringComparison.OrdinalIgnoreCase)))
        {
            var index = productsGrid.Rows.Add(product.Sku, product.Name,
                product.Available == 0 ? "Нет в наличии" : product.Available,
                $"{product.Price:N2} ₽", product.Returnable ? "Возвратная" : "Нет");
            productsGrid.Rows[index].Tag = product;
            if (product.Available == 0)
                productsGrid.Rows[index].DefaultCellStyle.ForeColor = Infrastructure.UITheme.TextMuted;
        }
    }

    private void AddSelectedProduct()
    {
        if (productsGrid.CurrentRow?.Tag is not Product product)
        {
            MessageBox.Show("Выберите товар.", "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (product.Available == 0)
        {
            MessageBox.Show("Этого товара нет в наличии на выбранном складе. Выберите другой товар или адрес.",
                "Нет в наличии", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        foreach (DataGridViewRow row in cartGrid.Rows)
        {
            if (row.Tag is Product existing && existing == product)
            {
                var quantity = Convert.ToInt32(row.Cells[1].Value) + 1;
                if (quantity > product.Available)
                {
                    MessageBox.Show($"Доступно только {product.Available} единиц.", "Недостаточно товара");
                    return;
                }
                UpdateCartRow(row, product, quantity);
                UpdateTotal();
                return;
            }
        }
        var index = cartGrid.Rows.Add();
        var newRow = cartGrid.Rows[index];
        newRow.Tag = product;
        UpdateCartRow(newRow, product, 1);
        UpdateTotal();
    }

    private void RemoveSelectedProduct()
    {
        if (cartGrid.CurrentRow is not { } row)
            return;
        var quantity = Convert.ToInt32(row.Cells[1].Value);
        if (row.Tag is Product product && quantity > 1)
            UpdateCartRow(row, product, quantity - 1);
        else
            cartGrid.Rows.Remove(row);
        UpdateTotal();
    }

    private static void UpdateCartRow(DataGridViewRow row, Product product, int quantity)
    {
        row.Cells[0].Value = product.Name;
        row.Cells[1].Value = quantity;
        row.Cells[2].Value = $"{product.Price:N2} ₽";
        row.Cells[3].Value = $"{product.Price * quantity:N2} ₽";
    }

    private void UpdateTotal()
    {
        decimal total = 0;
        foreach (DataGridViewRow row in cartGrid.Rows)
        {
            if (row.Tag is Product product)
                total += product.Price * Convert.ToInt32(row.Cells[1].Value);
        }
        totalLabel.Text = $"Итого: {total:N2} ₽";
    }

    private void searchTextBox_TextChanged(object? sender, EventArgs e) => LoadProducts(searchTextBox.Text);
    private void addButton_Click(object? sender, EventArgs e) => AddSelectedProduct();
    private void removeButton_Click(object? sender, EventArgs e) => RemoveSelectedProduct();
    private void productsGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e) => AddSelectedProduct();

    private void checkoutButton_Click(object? sender, EventArgs e)
    {
        if (cartGrid.Rows.Count == 0)
        {
            MessageBox.Show("Сначала добавьте товары в корзину.", "Корзина пуста", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        using var form = new OrderEditForm(UserRole.Client);
        form.ShowDialog(this);
    }
}
