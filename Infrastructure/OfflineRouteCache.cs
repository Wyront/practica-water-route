using System.Security.Cryptography;
using WaterRoute.Models;

namespace WaterRoute.Infrastructure;

public sealed class OfflineRouteCache
{
    private const string Entropy = "WaterRoute.OfflineCache.v1";

    public async Task SaveRouteAsync(int routeId, string routeNumber, string orderNumber,
        string clientName, string address, double warehouseLat, double warehouseLon,
        double destLat, double destLon, string status, string timeWindow)
    {
        var data = new CachedRoute
        {
            RouteId = routeId,
            RouteNumber = routeNumber,
            OrderNumber = orderNumber,
            ClientName = clientName,
            Address = address,
            WarehouseLat = warehouseLat,
            WarehouseLon = warehouseLon,
            DestLat = destLat,
            DestLon = destLon,
            Status = status,
            TimeWindow = timeWindow,
            CachedAt = DateTime.UtcNow
        };

        var json = System.Text.Json.JsonSerializer.Serialize(data);
        var encrypted = ProtectedData.Protect(
            System.Text.Encoding.UTF8.GetBytes(json),
            System.Text.Encoding.UTF8.GetBytes(Entropy),
            DataProtectionScope.CurrentUser);

        await File.WriteAllBytesAsync(GetCachePath(routeId), encrypted);
    }

    public async Task<CachedRoute?> GetRouteAsync(int routeId)
    {
        var path = GetCachePath(routeId);
        if (!File.Exists(path)) return null;

        try
        {
            var encrypted = await File.ReadAllBytesAsync(path);
            var decrypted = ProtectedData.Unprotect(encrypted,
                System.Text.Encoding.UTF8.GetBytes(Entropy), DataProtectionScope.CurrentUser);
            var json = System.Text.Encoding.UTF8.GetString(decrypted);
            return System.Text.Json.JsonSerializer.Deserialize<CachedRoute>(json);
        }
        catch
        {
            return null;
        }
    }

    public async Task ClearCacheAsync(int routeId)
    {
        var path = GetCachePath(routeId);
        if (File.Exists(path)) File.Delete(path);
        await Task.CompletedTask;
    }

    private static string GetCachePath(int routeId)
    {
        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WaterRoute", "RouteCache");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, $"route_{routeId}.dat");
    }
}

public sealed class CachedRoute
{
    public int RouteId { get; init; }
    public string RouteNumber { get; init; } = "";
    public string OrderNumber { get; init; } = "";
    public string ClientName { get; init; } = "";
    public string Address { get; init; } = "";
    public double WarehouseLat { get; init; }
    public double WarehouseLon { get; init; }
    public double DestLat { get; init; }
    public double DestLon { get; init; }
    public string Status { get; init; } = "";
    public string TimeWindow { get; init; } = "";
    public DateTime CachedAt { get; init; }
}