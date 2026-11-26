namespace WaterRoute.Models;

public sealed record LoginRequest(string Email, string Password, bool RememberMe);

public sealed record LoginResult(bool Success, UserSession? Session, string? ErrorMessage);

public sealed record RegisterRequest(string FullName, string Email, string Phone, string Password, string ConfirmPassword);

public sealed record SessionToken(string Token, DateTime ExpiresAt);

public sealed record ChangePasswordRequest(string OldPassword, string NewPassword, string ConfirmPassword);