namespace WaterRoute.Models;

public sealed record LoginRequest(string Email, string Password, bool RememberMe);

public sealed record LoginResult(bool Success, UserSession? Session, string? ErrorMessage);

public sealed record RegisterRequest(string FullName, string Email, string Phone, string Password, string ConfirmPassword);

public sealed record SessionToken(string Token, DateTime ExpiresAt, int UserId = 0);

// Строка результата sp_auth_login / sp_auth_validate_session.
// Имена колонок сопоставляются со свойствами без учёта регистра и символов "_" (user_id -> UserId).
public sealed class UserAuthRow
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}

public sealed record ChangePasswordRequest(string OldPassword, string NewPassword, string ConfirmPassword);