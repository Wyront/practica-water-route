using Microsoft.Extensions.DependencyInjection;
using WaterRoute.Forms;
using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Controls;

public static class PageFactory
{
    public static Control Create(PageId id, UserSession session, ICurrentUser? currentUser = null, AuthService? authService = null, IServiceProvider? services = null) => (id, session.Role) switch
    {
        // Client-specific forms (DI)
        (PageId.Profile, UserRole.Client) => services!.GetRequiredService<ClientProfileForm>(),
        (PageId.Orders, UserRole.Client) => services!.GetRequiredService<ClientOrderHistoryForm>(),
        (PageId.Routes, UserRole.Client) => services!.GetRequiredService<ClientDeliveryTrackingForm>(),

        // Dispatcher-specific forms (DI)
        (PageId.Clients, UserRole.Dispatcher) => services!.GetRequiredService<DispatcherClientForm>(),
        (PageId.Orders, UserRole.Dispatcher) => services!.GetRequiredService<DispatcherOrderForm>(),
        (PageId.Routes, UserRole.Dispatcher) => services!.GetRequiredService<DispatcherRouteForm>(),
        (PageId.Monitor, UserRole.Dispatcher) => services!.GetRequiredService<DispatcherMonitorForm>(),

        // Storekeeper-specific forms (DI)
        (PageId.Stock, UserRole.Storekeeper) => services!.GetRequiredService<StorekeeperStockForm>(),
        (PageId.Containers, UserRole.Storekeeper) => services!.GetRequiredService<StorekeeperContainersForm>(),
        

        // Courier-specific forms (DI)
        (PageId.Routes, UserRole.Courier) => services!.GetRequiredService<CourierRoutesForm>(),
        (PageId.Profile, UserRole.Courier) => services!.GetRequiredService<CourierHistoryForm>(),
        (PageId.Chat, UserRole.Courier) => services!.GetRequiredService<CourierChatForm>(),
        (PageId.Problems, UserRole.Courier) => services!.GetRequiredService<CourierProblemsForm>(),
        (PageId.Reviews, UserRole.Courier) => services!.GetRequiredService<CourierReviewsForm>(),

        // Map for all roles
        (PageId.Map, _) => services!.GetRequiredService<MapViewForm>(),

        // General forms
        (PageId.Profile, _) => new ProfilePage(session),
        (PageId.Dashboard, _) => new DashboardPage(session, currentUser!, authService, services?.GetService<IRepository>()),
        (PageId.Catalog, _) => new CatalogPage(),
        (PageId.Calendar, _) => new CalendarPage(),
        (PageId.Chat, _) => new ChatPage(session),
        (PageId.Monitor, _) => new MonitorPage(),
        (PageId.Directories, _) => new RegistryPage(CreateDirectoriesOptions(session)),
        (PageId.Users, _) => new RegistryPage(CreateUsersOptions(session)),
        (PageId.Clients, _) => new RegistryPage(CreateClientsOptions(session)),
        (PageId.Orders, _) => new RegistryPage(CreateOrdersOptions(session)),
        (PageId.Routes, _) => new RegistryPage(CreateRoutesOptions(session)),
        (PageId.Stock, _) => new RegistryPage(CreateStockOptions(session)),
        (PageId.Containers, _) => new RegistryPage(CreateContainersOptions(session)),
        (PageId.Problems, _) => new RegistryPage(CreateProblemsOptions(session)),
        (PageId.Reviews, _) => new RegistryPage(CreateReviewsOptions(session)),
        (PageId.Reports, _) => new ReportsPage(session),
        (PageId.Notifications, _) => new NotificationsPage(),
        _ => new DashboardPage(session, currentUser!, authService, services?.GetService<IRepository>())
    };

    private static RegistryOptions CreateDirectoriesOptions(UserSession session) => new()
    {
        Title = "Справочники",
        Subtitle = "Просмотр способов оплаты, типов воды, товаров, складов и автомобилей",
        Columns = ["Таблица", "Записей"],
        Rows =
        [
            ["Способы оплаты", "4"],
            ["Типы воды и тары", "3"],
            ["Товары", "6"],
            ["Склады", "3"],
            ["Автомобили", "10"]
        ],
        SearchPlaceholder = "Поиск",
        FilterLabel = "",
        CanView = false
    };

    private static RegistryOptions CreateUsersOptions(UserSession session) => new()
    {
        Title = "Пользователи",
        Subtitle = "Реестр пользователей и ролей",
        Columns = ["ФИО", "E-mail", "Телефон", "Роль", "Активен"],
        Rows =
        [
            ["Администратор", "admin@water.test", "+79000000001", "Администратор", "Да"],
            ["Диспетчер Анна", "disp@water.test", "+79000000002", "Диспетчер", "Да"],
            ["Кладовщик Дмитрий", "wh@water.test", "+79000000003", "Кладовщик", "Да"],
            ["Курьер Алексей", "courier1@water.test", "+79000000004", "Курьер", "Да"],
            ["Клиент №1", "client1@water.test", "+79000000011", "Клиент", "Да"]
        ],
        SearchPlaceholder = "Поиск по ФИО, e-mail или телефону",
        FilterLabel = "Активен:",
        CanEdit = true,
        CanView = true
    };

    private static RegistryOptions CreateClientsOptions(UserSession session) => new()
    {
        Title = "Клиенты",
        Subtitle = "Клиенты и их адресная книга",
        Columns = ["Клиент", "Телефон", "Адресов", "Заказов", "Активен"],
        Rows =
        [
            ["Клиент №1", "+79000000011", "2", "8", "Да"],
            ["ООО «Ручеёк»", "+79000000012", "1", "5", "Да"],
            ["Кафе «Нева»", "+79000000013", "3", "12", "Да"]
        ],
        SearchPlaceholder = "Поиск по клиенту или телефону",
        FilterLabel = "Активен:",
        CanCreate = true,
        CanEdit = true,
        CanDelete = true
    };

    private static RegistryOptions CreateOrdersOptions(UserSession session) => new()
    {
        Title = session.Role == UserRole.Client ? "Мои заказы" : "Заказы",
        Subtitle = "Реестр заказов, резервирования и автоматического распределения",
        Columns = ["Заказ", "Клиент", "Адрес", "Склад", "Дата", "Окно", "Статус"],
        Rows =
        [
            ["WR-10241", "Клиент №1", "Невский проспект, 10", "Центральный", "28.11.2026", "12:00–15:00", "Запланирован"],
            ["WR-10242", "Анна Воронова", "Приморский проспект, 25", "Северный", "28.11.2026", "15:00–18:00", "Новый"],
            ["WR-10243", "Кафе «Север»", "ул. Озёрная, 4", "Приморский", "27.11.2026", "09:00–12:00", "Доставлен"]
        ],
        SearchPlaceholder = "Поиск по номеру заказа или клиенту",
        FilterLabel = "Статус:",
        CanCreate = session.Role is UserRole.Client or UserRole.Dispatcher,
        CanEdit = session.Role is UserRole.Client or UserRole.Dispatcher,
        CanView = true,
        CanDelete = session.Role == UserRole.Dispatcher,
        DeleteText = "Отменить заказ"
    };

    private static RegistryOptions CreateRoutesOptions(UserSession session) => new()
    {
        Title = session.Role == UserRole.Courier ? "Мои маршруты" : "Маршруты",
        Subtitle = "Один заказ — один маршрут — одна остановка",
        Columns = ["Маршрут", "Заказ", "Дата", "Окно", "Склад", "Курьер", "Автомобиль", "Статус"],
        Rows =
        [
            ["R-021", "WR-10241", "28.11.2026", "12:00–15:00", "Центральный", "Курьер Алексей", "А001АА78", "Запланирован"],
            ["R-020", "WR-10240", "27.11.2026", "09:00–12:00", "Центральный", "Курьер Алексей", "А001АА78", "Завершён"],
            ["R-022", "WR-10242", "28.11.2026", "15:00–18:00", "Северный", "Курьер Сергей", "А002АА78", "Запланирован"]
        ],
        SearchPlaceholder = "Поиск по маршруту, заказу или курьеру",
        FilterLabel = "Статус:",
        CanEdit = session.Role is UserRole.Dispatcher or UserRole.Courier,
        CanView = true,
        AuxiliaryText = session.Role == UserRole.Courier ? "Обработать доставку" : "Открыть карточку"
    };

    private static RegistryOptions CreateStockOptions(UserSession session) => new()
    {
        Title = "Склад",
        Subtitle = "Физический остаток, резерв и доступное количество",
        Columns = ["Артикул", "Товар", "Склад", "Физический", "Резерв", "Доступно", "Минимум", "Статус"],
        Rows =
        [
            ["WT-019", "Вода питьевая 19 л", "Центральный", "120", "36", "84", "50", "Достаточно"],
            ["WT-005", "Вода питьевая 5 л", "Центральный", "160", "34", "126", "40", "Достаточно"],
            ["AC-001", "Помпа механическая", "Центральный", "20", "8", "12", "10", "Низкий остаток"]
        ],
        SearchPlaceholder = "Поиск по товару или складу",
        FilterLabel = "Статус:",
        CanCreate = session.Role is UserRole.Storekeeper,
        CreateText = "Складская операция",
        CanEdit = session.Role is UserRole.Storekeeper,
        EditText = "Просмотреть журнал",
        CanView = true
    };

    private static RegistryOptions CreateContainersOptions(UserSession session) => new()
    {
        Title = "Возвратная тара",
        Subtitle = "Выдача, возврат и долги по бутылям",
        Columns = ["Маршрут", "Клиент", "Тип тары", "Выдано", "Возвращено", "Ожидает", "Долг", "Статус"],
        Rows =
        [
            ["R-021", "Клиент №1", "Бутыль 19 л", "4", "0", "0", "4", "Все у клиента"],
            ["R-020", "Анна Воронова", "Бутыль 19 л", "6", "4", "2", "2", "Ожидает приёмку"],
            ["R-018", "Кафе «Север»", "Бутыль 19 л", "8", "8", "0", "0", "Возвращено"]
        ],
        SearchPlaceholder = "Поиск по маршруту или клиенту",
        FilterLabel = "Статус:",
        CanEdit = session.Role is UserRole.Storekeeper or UserRole.Courier,
        CanView = true,
        AuxiliaryText = session.Role switch
        {
            UserRole.Storekeeper => "Подтвердить приём",
            UserRole.Courier => "Зафиксировать возврат",
            _ => null
        }
    };

    private static RegistryOptions CreateProblemsOptions(UserSession session) => new()
    {
        Title = "Проблемы",
        Subtitle = "Регистрация и обработка проблем доставки",
        Columns = ["Дата", "Маршрут", "Категория", "Автор", "Статус", "Решение"],
        Rows =
        [
            ["25.11.2026 09:20", "R-021", "Отсутствует товар", "Курьер Алексей", "В работе", "—"],
            ["25.11.2026 08:40", "R-020", "Клиент не отвечает", "Диспетчер Анна", "Решена", "Перенос на 15:00–18:00"],
            ["24.11.2026 18:10", "R-018", "Повреждение тары", "Курьер Алексей", "Решена", "Списание 1 бутыли"]
        ],
        SearchPlaceholder = "Поиск по маршруту или категории",
        FilterLabel = "Статус:",
        FilterColumnIndex = 4, // Status column (0-based index)
        CanCreate = session.Role is UserRole.Courier or UserRole.Dispatcher,
        CanEdit = session.Role is UserRole.Dispatcher,
        DeleteText = "Закрыть проблему"
    };

    private static RegistryOptions CreateReviewsOptions(UserSession session) => new()
    {
        Title = session.Role == UserRole.Courier ? "Мои отзывы" : "Отзывы",
        Subtitle = "Независимые оценки курьера и заказа",
        Columns = ["Заказ", "Клиент", "Оценка курьера", "Отзыв о курьере", "Оценка заказа", "Отзыв о заказе"],
        Rows =
        [
            ["WR-10240", "Анна Воронова", "5", "Всё отлично", "4", "Быстрая доставка"],
            ["WR-10239", "ООО «Ручеёк»", "5", "Вежливый курьер", "5", "Вода качественная"],
            ["WR-10238", "Кафе «Север»", "4", "Без комментариев", "5", "Стабильно по утрам"]
        ],
        SearchPlaceholder = "Поиск по заказу или клиенту",
        FilterLabel = "Оценка курьера:",
        FilterColumnIndex = 2, // "Оценка курьера" column (0-based index)
        CanEdit = session.Role == UserRole.Client,
        EditText = "Изменить отзыв"
    };

    private static RegistryOptions CreateNotificationsOptions(UserSession session) => new()
    {
        Title = "Уведомления",
        Subtitle = "Внутренние уведомления о важных событиях",
        Columns = ["Дата", "Событие", "Тип", "Новое"],
        Rows =
        [
            ["25.11.2026 09:45", "R-021 · курьер прибыл по адресу", "Доставка", "Да"],
            ["25.11.2026 09:20", "WR-10241 · маршрут назначен", "Маршрут", "Нет"],
            ["25.11.2026 08:55", "Вода питьевая 19 л · низкий остаток", "Склад", "Да"],
            ["25.11.2026 08:40", "Новое сообщение от диспетчера", "Чат", "Да"]
        ],
        SearchPlaceholder = "Поиск по событию",
        FilterLabel = "Новое:",
        CanView = false
    };
}