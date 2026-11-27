using System.Text.RegularExpressions;
using MySqlConnector;

namespace WaterRoute.Infrastructure;

/// <summary>Общие проверки ввода и показ ошибок пользователю.</summary>
public static class UiErrors
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$", RegexOptions.Compiled);
    private static readonly Regex PhoneRegex = new(@"^\+?[0-9][0-9\s\-\(\)]{9,19}$", RegexOptions.Compiled);

    public const int MinPasswordLength = 8;

    public static bool IsValidEmail(string? value) => !string.IsNullOrWhiteSpace(value) && EmailRegex.IsMatch(value.Trim());

    /// <summary>Телефон необязателен: пустая строка считается корректной.</summary>
    public static bool IsValidPhone(string? value) => string.IsNullOrWhiteSpace(value) || PhoneRegex.IsMatch(value.Trim());

    public static string? NormalizePhone(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public static string Describe(Exception ex) => ex switch
    {
        // SIGNAL SQLSTATE '45000' — бизнес-ошибка из процедуры, текст уже понятен пользователю
        MySqlException { SqlState: "45000" } m => m.Message,
        MySqlException m => $"Ошибка базы данных: {m.Message}",
        _ => ex.Message
    };

    public static void Show(Exception ex, IWin32Window? owner = null) =>
        MessageBox.Show(owner, Describe(ex), "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public static void Warn(string message, IWin32Window? owner = null) =>
        MessageBox.Show(owner, message, "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public static void Info(string message, IWin32Window? owner = null) =>
        MessageBox.Show(owner, message, "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
