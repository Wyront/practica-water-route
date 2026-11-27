using MySqlConnector;
using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Services;

public sealed class AuthService
{
    private const string InvalidCredentials = "Неверный e-mail или пароль";

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
            return Fail("Введите e-mail и пароль");

        try
        {
            // Пароль в БД не отправляем: сверка bcrypt выполняется в приложении,
            // параметр p_password_plain процедурой не используется.
            var row = await _repo.QuerySingleAsync<UserAuthRow>("sp_auth_login",
                new { p_email = request.Email.Trim(), p_password_plain = string.Empty }, ct);

            if (row == null || !VerifyPassword(request.Password, row.PasswordHash))
                return Fail(InvalidCredentials);

            if (!Enum.TryParse<UserRole>(row.RoleCode, true, out var role))
                return Fail($"Неизвестная роль пользователя: {row.RoleCode}");

            var session = new UserSession(row.UserId, row.FullName, row.Email, role);
            await _sessionStore.SaveSessionAsync(session, request.RememberMe, ct);
            return new LoginResult(true, session, null);
        }
        catch (MySqlException ex) when (ex.SqlState == "45000")
        {
            // Ошибки бизнес-логики из SIGNAL. "Пользователь не найден" не раскрываем.
            return Fail(ex.Message.Contains("не найден", StringComparison.OrdinalIgnoreCase) ? InvalidCredentials : ex.Message);
        }
        catch (MySqlException ex)
        {
            return Fail($"Нет связи с базой данных: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Fail($"Ошибка входа: {ex.Message}");
        }
    }

    public async Task<LoginResult> TryAutoLoginAsync(CancellationToken ct = default)
    {
        var token = await _sessionStore.GetSessionAsync(ct);
        if (token == null)
            return new LoginResult(false, null, null);

        if (token.UserId <= 0)
        {
            await _sessionStore.ClearSessionAsync(ct);
            return new LoginResult(false, null, null);
        }

        try
        {
            var row = await _repo.QuerySingleAsync<UserAuthRow>("sp_auth_validate_session",
                new { p_user_id = token.UserId, p_session_token = token.Token }, ct);

            if (row == null || !Enum.TryParse<UserRole>(row.RoleCode, true, out var role))
            {
                await _sessionStore.ClearSessionAsync(ct);
                return new LoginResult(false, null, null);
            }

            return new LoginResult(true, new UserSession(row.UserId, row.FullName, row.Email, role), null);
        }
        catch (MySqlException ex) when (ex.SqlState == "45000")
        {
            // Пользователь деактивирован или удалён — сессия недействительна.
            await _sessionStore.ClearSessionAsync(ct);
            return new LoginResult(false, null, null);
        }
        catch
        {
            // БД недоступна — сохранённую сессию не трогаем, просто показываем форму входа.
            return new LoginResult(false, null, null);
        }
    }

    /// <summary>Самостоятельная регистрация клиента. Возвращает (успех, текст ошибки).</summary>
    public async Task<(bool Ok, string? Error)> RegisterClientAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var fullName = request.FullName?.Trim() ?? string.Empty;
        var email = request.Email?.Trim() ?? string.Empty;

        if (fullName.Length < 2)
            return (false, "Введите ФИО");
        if (!UiErrors.IsValidEmail(email))
            return (false, "Введите корректный e-mail");
        if (!UiErrors.IsValidPhone(request.Phone))
            return (false, "Телефон указан в неверном формате");
        if (string.IsNullOrEmpty(request.Password) || request.Password.Length < UiErrors.MinPasswordLength)
            return (false, $"Пароль должен быть не короче {UiErrors.MinPasswordLength} символов");
        if (request.Password != request.ConfirmPassword)
            return (false, "Пароль и подтверждение не совпадают");

        var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        try
        {
            var output = await _repo.ExecuteWithOutputsAsync("sp_client_register",
                new
                {
                    full_name = fullName,
                    email,
                    phone = UiErrors.NormalizePhone(request.Phone),
                    password_plain = hash
                },
                new[] { "p_user_id" }, ct);

            return output.TryGetValue("p_user_id", out var id) && id != null
                ? (true, null)
                : (false, "Не удалось создать учётную запись");
        }
        catch (Exception ex)
        {
            return (false, UiErrors.Describe(ex));
        }
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        await _sessionStore.ClearSessionAsync(ct);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        try
        {
            return !string.IsNullOrEmpty(hash) && BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (Exception)
        {
            // Некорректный формат хеша в БД.
            return false;
        }
    }

    private static LoginResult Fail(string message) => new(false, null, message);
}
