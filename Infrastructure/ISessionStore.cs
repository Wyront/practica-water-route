using WaterRoute.Models;

namespace WaterRoute.Infrastructure;

public interface ISessionStore
{
    Task<SessionToken?> GetSessionAsync(CancellationToken ct = default);
    Task SaveSessionAsync(UserSession session, bool rememberMe, CancellationToken ct = default);
    Task ClearSessionAsync(CancellationToken ct = default);
}