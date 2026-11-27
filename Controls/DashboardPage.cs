using System.Threading;
using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Controls;

public sealed partial class DashboardPage : BasePage
{
    public event EventHandler<PageId>? NavigationRequested;

    private sealed record Metric(string Title, string Value, string Note, PageId Target);
    private readonly AuthService? _authService;
    private readonly ICurrentUser? _currentUser;
    private readonly IRepository? _repo;
    private CancellationTokenSource? _loadCts;
    private int _loadVersion;

    public DashboardPage() : this(RoleInfo.CreateDemoSession(UserRole.Admin), null!, null, null)
    {
    }

    public DashboardPage(UserSession session, ICurrentUser? currentUser = null,
        AuthService? authService = null, IRepository? repo = null)
        : base("Главная", GetSubtitle(session.Role))
    {
        _currentUser = currentUser;
        _authService = authService;
        _repo = repo;
        InitializeComponent();
        ConfigureMetrics(GetMetrics(session.Role));
        processLabel.Text = GetProcessText(session.Role);
        Load += DashboardPage_Load;
    }

    private async void DashboardPage_Load(object? sender, EventArgs e)
    {
        await LoadDashboardDataAsync();
    }

    public async Task LoadDashboardDataAsync()
    {
        if (_repo == null || _currentUser?.Session == null) return; // демо — статичные цифры

        var version = Interlocked.Increment(ref _loadVersion);
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        _loadCts = new CancellationTokenSource();
        var ct = _loadCts.Token;

        try
        {
            var m = await _repo.QuerySingleAsync<dynamic>("sp_dashboard_metrics",
                new { user_id = _currentUser.Session.Id }, ct);
            if (m == null || version != _loadVersion || ct.IsCancellationRequested || IsDisposed) return;

            string?[] values = _currentUser.Session.Role switch
            {
                UserRole.Admin => [S(m.active_users), S(m.orders_today), S(m.active_routes), S(m.open_problems), Money(m.revenue_month)],
                UserRole.Dispatcher => [S(m.new_orders), S(m.my_active_routes), S(m.in_delivery), S(m.open_problems), S(m.low_stock)],
                UserRole.Storekeeper => [S(m.low_stock), S(m.receipts_today), S(m.pending_returns), S(m.movements_today), S(m.container_debt)],
                UserRole.Courier => [m.current_route_id == null ? "—" : $"R-{m.current_route_id}", S(m.completed_deliveries), S(m.open_problems), Convert.ToDecimal(m.avg_rating).ToString("F1"), S(m.unread_messages)],
                UserRole.Client => [S(m.active_orders), m.next_route_id == null ? "—" : $"R-{m.next_route_id}", S(m.container_debt), S(m.unread_notifications), "+"],
                _ => []
            };

            if (version != _loadVersion || ct.IsCancellationRequested) return;
            var labels = new[] { metricValue1, metricValue2, metricValue3, metricValue4, metricValue5 };
            for (var i = 0; i < labels.Length && i < values.Length; i++)
                if (values[i] != null) labels[i].Text = values[i];
        }
        catch (OperationCanceledException)
        {
            // Более новая загрузка отменила текущую.
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Dashboard load error: {ex.Message}");
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _loadCts?.Cancel();
            _loadCts?.Dispose();
        }
        base.Dispose(disposing);
    }

    private static string S(object? v) => v == null ? "—" : Convert.ToString(v) ?? "—";
    private static string Money(object? v) => v == null ? "—" : $"{Convert.ToDecimal(v):N0} ₽";

    private void ConfigureMetrics(IReadOnlyList<Metric> metrics)
    {
        var cards = new[] { metricCard1, metricCard2, metricCard3, metricCard4, metricCard5 };
        var titles = new[] { metricTitle1, metricTitle2, metricTitle3, metricTitle4, metricTitle5 };
        var values = new[] { metricValue1, metricValue2, metricValue3, metricValue4, metricValue5 };
        var notes = new[] { metricNote1, metricNote2, metricNote3, metricNote4, metricNote5 };
        for (var i = 0; i < cards.Length; i++)
        {
            var metric = metrics[i];
            titles[i].Text = metric.Title;
            values[i].Text = metric.Value;
            notes[i].Text = metric.Note;
            cards[i].Tag = metric.Target;
            foreach (Control control in cards[i].Controls)
                control.Tag = metric.Target;
        }
    }

    private void metricCard_Click(object? sender, EventArgs e)
    {
        if (sender is Control { Tag: PageId target })
            NavigationRequested?.Invoke(this, target);
    }

    private static string GetSubtitle(UserRole role) => role switch
    {
        UserRole.Admin => "Администрирование, мониторинг данных и аналитика системы",
        UserRole.Dispatcher => "Очередь заказов, назначение ресурсов и контроль доставок",
        UserRole.Storekeeper => "Остатки, движения товаров и приём возвратной тары",
        UserRole.Courier => "Ваш текущий маршрут, история доставок и связь с диспетчером",
        UserRole.Client => "Оформление заказов и отслеживание ваших доставок",
        _ => string.Empty
    };

    private static string GetProcessText(UserRole role) => role switch
    {
        UserRole.Client => "1. Выберите адрес\n2. Добавьте товары в корзину\n3. Оформите заказ\n4. Дождитесь автоматического назначения\n5. Отслеживайте доставку",
        UserRole.Courier => "1. Откройте текущий маршрут\n2. Начните выполнение\n3. Отметьте прибытие\n4. Подтвердите полную доставку\n5. Зафиксируйте возврат тары",
        UserRole.Storekeeper => "1. Проверьте низкие остатки\n2. Зарегистрируйте поступления\n3. Просмотрите ожидающие возвраты\n4. Подтвердите приём тары\n5. Сформируйте отчёт",
        UserRole.Dispatcher => "1. Проверьте очередь заказов\n2. Контролируйте свободные ресурсы\n3. Корректируйте назначение при необходимости\n4. Обрабатывайте проблемы\n5. Отвечайте курьерам в чате",
        _ => "1. Управляйте пользователями\n2. Поддерживайте справочники\n3. Просматривайте заказы и маршруты\n4. Анализируйте проблемы\n5. Формируйте отчёты"
    };

    private static List<Metric> GetMetrics(UserRole role) => role switch
    {
        UserRole.Admin =>
        [
            new("Пользователи", "28", "25 активных", PageId.Users),
            new("Заказы сегодня", "12", "+3 за последний час", PageId.Orders),
            new("Активные маршруты", "1", "один диспетчер занят", PageId.Routes),
            new("Открытые проблемы", "3", "1 требует внимания", PageId.Problems),
            new("Выручка за месяц", "248 600 ₽", "по полным доставкам", PageId.Reports)
        ],
        UserRole.Dispatcher =>
        [
            new("Новые заказы", "7", "ожидают ресурсов", PageId.Orders),
            new("Активный маршрут", "1", "R-021 · в работе", PageId.Routes),
            new("В доставке", "1", "прибытие отмечено", PageId.Monitor),
            new("Проблемы", "3", "2 открытые", PageId.Problems),
            new("Низкие остатки", "4", "на двух складах", PageId.Reports)
        ],
        UserRole.Storekeeper =>
        [
            new("Низкие остатки", "4", "ниже минимума", PageId.Stock),
            new("Поступления сегодня", "12", "+240 единиц", PageId.Stock),
            new("Ожидают приёмки", "3", "возвраты от курьеров", PageId.Containers),
            new("Движения за день", "18", "все подтверждены", PageId.Stock),
            new("Долг по таре", "26", "бутылей у клиентов", PageId.Containers)
        ],
        UserRole.Courier =>
        [
            new("Текущий маршрут", "R-021", "12:00–15:00", PageId.Routes),
            new("Доставка", "1", "Невский проспект, 10", PageId.Map),
            new("Проблемы", "1", "ожидает решения", PageId.Problems),
            new("Средний рейтинг", "4,8", "по 16 оценкам", PageId.Reviews),
            new("Новые сообщения", "2", "от диспетчера", PageId.Chat)
        ],
        UserRole.Client =>
        [
            new("Активные заказы", "2", "один ожидает ресурсов", PageId.Orders),
            new("Ближайшая доставка", "12:00", "сегодня, 12:00–15:00", PageId.Routes),
            new("Долг по таре", "4", "бутыли 19 л", PageId.Profile),
            new("Уведомления", "3", "непрочитанных", PageId.Notifications),
            new("Новый заказ", "+", "перейти в каталог", PageId.Catalog)
        ],
        _ => []
    };
}
