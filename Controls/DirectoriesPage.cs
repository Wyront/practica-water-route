using WaterRoute.Forms;

namespace WaterRoute.Controls;

public partial class DirectoriesPage : BasePage
{
    public DirectoriesPage() : base(
        "Справочники",
        "Администрирование товаров, складов, автомобилей и вспомогательных списков")
    {
        InitializeComponent();
        ConfigurePages();
    }

    private void ConfigurePages()
    {
        productsPage.SetOptions(new RegistryOptions
        {
            Title = "Товары", Subtitle = "Артикулы, цены, условные места и возвратная тара",
            Columns = ["Артикул", "Наименование", "Цена", "Мест", "Тара", "Активен"],
            Rows = [["WT-019", "Вода питьевая 19 л", "250,00 ₽", "2", "Возвратная", "Да"], ["WT-005", "Вода питьевая 5 л", "90,00 ₽", "1", "Нет", "Да"], ["AC-001", "Помпа механическая", "450,00 ₽", "1", "Нет", "Да"]],
            FilterLabel = "Активен:", CanCreate = true, CanEdit = true, CanDelete = true
        });
        warehousesPage.SetOptions(new RegistryOptions
        {
            Title = "Склады", Subtitle = "Адреса, координаты и активность складов",
            Columns = ["Название", "Адрес", "Координаты", "Активен"],
            Rows = [["Центральный склад", "Складская улица, 1", "59.934300, 30.335100", "Да"], ["Северный склад", "Северная улица, 25", "60.015200, 30.320400", "Да"], ["Приморский склад", "Приморский проспект, 40", "59.985100, 30.210800", "Да"]],
            FilterLabel = "Активен:", CanCreate = true, CanEdit = true, CanDelete = true
        });
        vehiclesPage.SetOptions(new RegistryOptions
        {
            Title = "Автомобили", Subtitle = "Госномера и вместимость в условных местах",
            Columns = ["Госномер", "Модель", "Вместимость", "Активен"],
            Rows = [["А001АА78", "Ford Transit", "100", "Да"], ["А002АА78", "Ford Transit", "120", "Да"], ["В003ВВ78", "ГАЗель Next", "90", "Нет"]],
            FilterLabel = "Активен:", CanCreate = true, CanEdit = true, CanDelete = true
        });
        paymentsPage.SetOptions(new RegistryOptions
        {
            Title = "Способы оплаты", Subtitle = "Справочные значения без подключения реальных платежей",
            Columns = ["Код", "Название", "Активен"], Rows = [["cash", "Наличные", "Да"], ["card", "Карта", "Да"], ["online", "Онлайн", "Да"], ["transfer", "Безнал", "Да"]],
            FilterLabel = "Активен:", CanView = false
        });
        waterTypesPage.SetOptions(new RegistryOptions
        {
            Title = "Типы воды и тары", Subtitle = "Типы используются в товарах и раздельном учёте возвратной тары",
            Columns = ["Название", "Объём, л", "Активен"], Rows = [["Бутыль 19 л", "19", "Да"], ["ПЭТ 5 л", "5", "Да"], ["Без тары", "0", "Да"]],
            FilterLabel = "Активен:", CanView = false
        });
        productsPage.CreateRequested += (_, _) => OpenForm(new ProductEditForm());
        productsPage.EditRequested += (_, _) => OpenForm(new ProductEditForm());
        warehousesPage.CreateRequested += (_, _) => OpenForm(new WarehouseEditForm());
        warehousesPage.EditRequested += (_, _) => OpenForm(new WarehouseEditForm());
        vehiclesPage.CreateRequested += (_, _) => OpenForm(new VehicleEditForm());
        vehiclesPage.EditRequested += (_, _) => OpenForm(new VehicleEditForm());
        foreach (var page in new[] { productsPage, warehousesPage, vehiclesPage })
            page.DeleteRequested += (_, _) => MessageBox.Show("Запись будет деактивирована после проверки зависимостей. Исторические данные не удаляются.", "Деактивация", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void OpenForm(Form form)
    {
        using (form) form.ShowDialog(this);
    }
}
