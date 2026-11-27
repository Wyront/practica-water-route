namespace WaterRoute.Models;

public enum UserRole
{
    Admin,
    Dispatcher,
    Storekeeper,
    Courier,
    Client
}

public sealed record UserSession(int Id, string FullName, string Email, UserRole Role);

public static class RoleInfo
{
    public static string GetName(UserRole role) => role switch
    {
        UserRole.Admin => "Администратор",
        UserRole.Dispatcher => "Диспетчер",
        UserRole.Storekeeper => "Кладовщик",
        UserRole.Courier => "Курьер",
        UserRole.Client => "Клиент",
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
    };

    public static UserSession CreateDemoSession(UserRole role) => role switch
    {
        UserRole.Admin => new UserSession(1, "Администратор", "admin@water.test", role),
        UserRole.Dispatcher => new UserSession(2, "Диспетчер Анна", "disp@water.test", role),
        UserRole.Storekeeper => new UserSession(3, "Кладовщик Дмитрий", "wh@water.test", role),
        UserRole.Courier => new UserSession(4, "Курьер Алексей", "courier1@water.test", role),
        UserRole.Client => new UserSession(6, "Клиент №1", "client1@water.test", role),
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
    };
}
