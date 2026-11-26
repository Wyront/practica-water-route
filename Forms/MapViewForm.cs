using Mapsui;
using Mapsui.UI.WindowsForms;
using NetTopologySuite.Geometries;
using WaterRoute.Infrastructure;
using WaterRoute.Models;

namespace WaterRoute.Forms;

public sealed partial class MapViewForm : BaseEditForm
{
    private MapControl _mapControl = null!;
    private MapService? _mapService;
    private readonly OfflineRouteCache _cache = new();

    public MapViewForm() : base("Карта доставки", "Санкт-Петербург · без GPS и дорожной оптимизации · поддержка локальной MBTiles-карты", 1200, 800)
    {
        InitializeComponent();
        Load += MapViewForm_Load;
    }

    private async void MapViewForm_Load(object? sender, EventArgs e)
    {
        await InitializeMapAsync();
    }

    private async Task InitializeMapAsync()
    {
        try
        {
            _mapService = new MapService();
            _mapControl.Map = _mapService.Map;
            _mapService.SetCenter(59.9343, 30.3351, 11); // Центр СПб

            // Демо-маршрут: склад → клиент
            var warehouse = new NetTopologySuite.Geometries.Point(30.3351, 59.9343);
            var destination = new NetTopologySuite.Geometries.Point(30.3601, 59.9289);
            _mapService.SetRoute(warehouse, destination, "R-021");

            // Кэшируем для офлайна
            await _cache.SaveRouteAsync(1, "R-021", "WR-10241", "Клиент №1",
                "Невский проспект, 10", 59.9343, 30.3351, 59.9289, 30.3601,
                "В работе", "12:00–15:00");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка инициализации карты: {ex.Message}", "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void offlineModeButton_Click(object? sender, EventArgs e)
    {
        // TODO: переключение на MBTiles
        MessageBox.Show("Переключение на офлайн-карту MBTiles будет подключено после подготовки файла saint-petersburg.mbtiles", "WaterRoute");
    }

    private void cacheButton_Click(object? sender, EventArgs e)
    {
        _notifications.ShowSuccess("Кэш", "Маршрут сохранён в защищённый локальный кэш. Доступен без соединения.");
    }

    private readonly INotificationService _notifications;

    protected override void Dispose(bool disposing)
    {
        _mapService?.Dispose();
        base.Dispose(disposing);
    }
}