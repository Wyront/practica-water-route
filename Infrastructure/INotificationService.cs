namespace WaterRoute.Infrastructure;

public interface INotificationService
{
    void ShowToast(string title, string message, ToastType type = ToastType.Info, int durationMs = 5000);
    void ShowError(string title, string message, int durationMs = 8000);
    void ShowWarning(string title, string message, int durationMs = 6000);
    void ShowSuccess(string title, string message, int durationMs = 4000);
}

public enum ToastType
{
    Info,
    Success,
    Warning,
    Error
}