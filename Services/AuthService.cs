using BCrypt.Net;
using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Services;

public sealed class AuthService
{
    private readonly IRepository _repo;
    private readonly ISessionStore _sessionStore;

    public AuthService(IRepository repo, ISessionStore sessionStore)
    {
        _repo = repo;
        _sessionStore = sessionStore;
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return new LoginResult(false, null, "Введите e-mail и пароль");

        try
        {
            await _repo.BeginTransactionAsync(ct);
            var result = await _repo.QuerySingleAsync<dynamic>("sp_auth_login",
                new { p_email = request.Email, p_password_plain = request.Password }, ct);

            if (result == null)
            {
                await _repo.RollbackAsync(ct);
                return new LoginResult(false, null, "Пользователь не найден");
            }

            var storedHash = (string)result.password_hash;
            if (!BCrypt.Net.BCrypt.Verify(request.Password, storedHash))
            {
                await _repo.RollbackAsync(ct);
                return new LoginResult(false, null, "Неверный пароль");
            }

            var roleCode = (string)result.role_code;
            var role = Enum.Parse<UserRole>(roleCode, true);

            var session = new UserSession(
                (int)result.user_id,
                (string)result.full_name,
                (string)result.email,
                role);

            await _repo.CommitAsync(ct);
            await _sessionStore.SaveSessionAsync(session, request.RememberMe, ct);

            return new LoginResult(true, session, null);
        }
        catch (Exception ex)
        {
            await _repo.RollbackAsync(ct);
            return new LoginResult(false, null, $"Ошибка входа: {ex.Message}");
        }
    }

    public async Task<LoginResult> TryAutoLoginAsync(CancellationToken ct = default)
    {
        var token = await _sessionStore.GetSessionAsync(ct);
        if (token == null)
            return new LoginResult(false, null, null);

        try
        {
            await _repo.BeginTransactionAsync(ct);
            await _repo.QuerySingleAsync<dynamic>("sp_auth_validate_session",
                new { p_user_id = 0, p_session_token = token.Token }, ct);

            await _repo.CommitAsync(ct);
            return new LoginResult(false, null, null);
        }
        catch
        {
            await _repo.RollbackAsync(ct);
            await _sessionStore.ClearSessionAsync(ct);
            return new LoginResult(false, null, null);
        }
    }

    public async Task<bool> RegisterClientAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
            return false;

        if (request.Password != request.ConfirmPassword)
            return false;

        var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        try
        {
            await _repo.BeginTransactionAsync(ct);
            await _repo.ExecuteAsync("sp_client_register", new
            {
                p_full_name = request.FullName,
                p_email = request.Email,
                p_phone = request.Phone ?? "",
                p_password_plain = hash
            }, ct);
            await _repo.CommitAsync(ct);
            return true;
        }
        catch
        {
            await _repo.RollbackAsync(ct);
            return false;
        }
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        await _sessionStore.ClearSessionAsync(ct);
    }
}