using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows.Forms;
using WaterRoute.Forms;
using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) => ShowFatal(e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, e) => ShowFatal(e.ExceptionObject as Exception);

        using var host = Host.CreateDefaultBuilder()
            .UseContentRoot(AppContext.BaseDirectory)
            .ConfigureServices((context, services) =>
            {
                // Настройки БД: appsettings.json, секция "Database".
                // Переопределяются переменными окружения, например Database__Password.
                var dbSettings = context.Configuration.GetSection("Database").Get<DatabaseSettings>()
                                 ?? new DatabaseSettings();
                services.AddWaterRouteServices(dbSettings);
                services.AddTransient<LoginForm>();
            })
            .Build();

        using var scope = host.Services.CreateScope();
        var loginForm = scope.ServiceProvider.GetRequiredService<LoginForm>();
        Application.Run(loginForm);
    }

    private static void ShowFatal(Exception? ex)
    {
        MessageBox.Show(
            ex?.Message ?? "Неизвестная ошибка",
            "WaterRoute — ошибка",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
