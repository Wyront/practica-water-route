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

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                var dbSettings = new DatabaseSettings
                {
                    Server = "localhost",
                    Port = 3306,
                    Database = "waterroute",
                    UserId = "root",
                    Password = "",
                    SslMode = false
                };
                services.AddWaterRouteServices(dbSettings);
                services.AddTransient<LoginForm>();
                services.AddTransient<MainForm>();
            })
            .Build();

        using var scope = host.Services.CreateScope();
        var loginForm = scope.ServiceProvider.GetRequiredService<LoginForm>();
        Application.Run(loginForm);
    }
}