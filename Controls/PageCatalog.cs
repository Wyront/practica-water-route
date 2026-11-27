using WaterRoute.Models;

namespace WaterRoute.Controls;

public sealed record NavigationItem(PageId Id, string Title);

public static class PageCatalog
{
    public static IReadOnlyList<NavigationItem> GetNavigation(UserRole role) => role switch
    {
        UserRole.Admin =>
        [
            new(PageId.Dashboard, "Главная"),
            new(PageId.Users, "Пользователи"),
            new(PageId.Directories, "Справочники"),
            new(PageId.Orders, "Заказы"),
            new(PageId.Routes, "Маршруты"),
            new(PageId.Stock, "Склад"),
            new(PageId.Containers, "Возвратная тара"),
            new(PageId.Problems, "Проблемы"),
            new(PageId.Reviews, "Отзывы"),
            new(PageId.Chat, "Чаты"),
            new(PageId.Reports, "Отчёты"),
            new(PageId.Notifications, "Уведомления")
        ],
        UserRole.Dispatcher =>
        [
            new(PageId.Dashboard, "Главная"),
            new(PageId.Orders, "Заказы"),
            new(PageId.Clients, "Клиенты"),
            new(PageId.Routes, "Маршруты"),
            new(PageId.Monitor, "Монитор"),
            new(PageId.Calendar, "Календарь"),
            new(PageId.Map, "Карта"),
            new(PageId.Chat, "Чаты"),
            new(PageId.Problems, "Проблемы"),
            new(PageId.Reports, "Отчёты"),
            new(PageId.Notifications, "Уведомления")
        ],
        UserRole.Storekeeper =>
        [
            new(PageId.Dashboard, "Главная"),
            new(PageId.Stock, "Склад"),
            new(PageId.Containers, "Возвратная тара"),
            new(PageId.Reports, "Отчёты"),
            new(PageId.Notifications, "Уведомления")
        ],
        UserRole.Courier =>
        [
            new(PageId.Dashboard, "Главная"),
            new(PageId.Routes, "Мои маршруты"),
            new(PageId.Map, "Карта"),
            new(PageId.Chat, "Чат с диспетчером"),
            new(PageId.Problems, "Проблемы"),
            new(PageId.Reviews, "Мои отзывы"),
            new(PageId.Notifications, "Уведомления")
        ],
        UserRole.Client =>
        [
            new(PageId.Dashboard, "Главная"),
            new(PageId.Catalog, "Каталог и корзина"),
            new(PageId.Orders, "Мои заказы"),
            new(PageId.Routes, "Моя доставка"),
            new(PageId.Map, "Карта доставки"),
            new(PageId.Reviews, "Мои отзывы"),
            new(PageId.Profile, "Профиль и адреса"),
            new(PageId.Notifications, "Уведомления")
        ],
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
    };
}
