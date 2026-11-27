using System.Security.Cryptography;
using WaterRoute.Models;

namespace WaterRoute.Infrastructure;

public sealed class SessionStore : ISessionStore
{
    private const string DataFile = "session.dat";
    private const string Entropy = "WaterRoute.Session.v1";

    public async Task<SessionToken?> GetSessionAsync(CancellationToken ct = default)
    {
        var path = GetDataPath();
        if (!File.Exists(path))
            return null;

        try
        {
            var encrypted = await File.ReadAllBytesAsync(path, ct);
            var decrypted = ProtectedData.Unprotect(encrypted, System.Text.Encoding.UTF8.GetBytes(Entropy), DataProtectionScope.CurrentUser);
            var json = System.Text.Encoding.UTF8.GetString(decrypted);
            var token = System.Text.Json.JsonSerializer.Deserialize<SessionToken>(json);
            if (token?.ExpiresAt < DateTime.UtcNow)
            {
                await ClearSessionAsync(ct);
                return null;
            }
            return token;
        }
        catch
        {
            await ClearSessionAsync(ct);
            return null;
        }
    }

    public async Task SaveSessionAsync(UserSession session, bool rememberMe, CancellationToken ct = default)
    {
        // Без "Запомнить меня" сессию на диск не сохраняем и стираем прежнюю.
        if (!rememberMe)
        {
            await ClearSessionAsync(ct);
            return;
        }

        var token = new SessionToken(
            Token: Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
            ExpiresAt: DateTime.UtcNow.AddDays(30),
            UserId: session.Id
        );

        var json = System.Text.Json.JsonSerializer.Serialize(token);
        var encrypted = ProtectedData.Protect(
            System.Text.Encoding.UTF8.GetBytes(json),
            System.Text.Encoding.UTF8.GetBytes(Entropy),
            DataProtectionScope.CurrentUser);

        await File.WriteAllBytesAsync(GetDataPath(), encrypted, ct);
    }

    public async Task ClearSessionAsync(CancellationToken ct = default)
    {
        var path = GetDataPath();
        if (File.Exists(path))
            File.Delete(path);
        await Task.CompletedTask;
    }

    private static string GetDataPath()
    {
        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WaterRoute");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "session.dat");
    }
}