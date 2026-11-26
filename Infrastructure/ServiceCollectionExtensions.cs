using Microsoft.Extensions.DependencyInjection;
using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Infrastructure;

using WaterRoute.Forms;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWaterRouteServices(this IServiceCollection services, DatabaseSettings dbSettings)
    {
        services.AddSingleton(dbSettings);
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<IRepository, Repository>();
        services.AddSingleton<ISessionStore, SessionStore>();
        services.AddScoped<AuthService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddTransient<RegistrationForm>();
        services.AddTransient<UserManagementForm>();
        services.AddTransient<DirectoryManagementForm>();
        services.AddTransient<AdminReportsForm>();
        services.AddTransient<PaymentMethodEditForm>();
        services.AddTransient<WaterTypeEditForm>();
        services.AddTransient<ClientProfileForm>();
        services.AddTransient<ClientOrderHistoryForm>();
        services.AddTransient<ClientDeliveryTrackingForm>();
        services.AddTransient<DispatcherClientForm>();
        services.AddTransient<DispatcherOrderForm>();
        services.AddTransient<DispatcherRouteForm>();
        services.AddTransient<DispatcherMonitorForm>();
        services.AddTransient<StorekeeperStockForm>();
        services.AddTransient<StorekeeperContainersForm>();
        services.AddTransient<StorekeeperImportForm>();
        services.AddTransient<StockReceiptForm>();
        services.AddTransient<StockAdjustmentForm>();
        services.AddTransient<StockJournalForm>();
        services.AddTransient<ContainerConfirmForm>();
        services.AddTransient<ContainerDebtForm>();
        services.AddTransient<CourierRoutesForm>();
        services.AddTransient<CourierDeliveryForm>();
        services.AddTransient<CourierHistoryForm>();
        services.AddTransient<CourierChatForm>();
        services.AddTransient<CourierProblemsForm>();
        services.AddTransient<CourierReviewsForm>();
        services.AddTransient<NumericInputForm>();
        services.AddTransient<MapViewForm>();
        services.AddSingleton<OfflineRouteCache>();
        services.AddScoped<INotificationService>(sp =>
        {
            var mainForm = sp.GetRequiredService<MainForm>();
            return new ToastNotificationService(mainForm);
        });
        return services;
    }
}

public interface ICurrentUser
{
    UserSession? Session { get; }
    bool IsAuthenticated { get; }
    UserRole? Role { get; }
    void SetSession(UserSession session);
    void Clear();
}

public sealed class CurrentUser : ICurrentUser
{
    public UserSession? Session { get; private set; }
    public bool IsAuthenticated => Session != null;
    public UserRole? Role => Session?.Role;

    public void SetSession(UserSession session) => Session = session;
    public void Clear() => Session = null;
}