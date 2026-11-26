using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.UI.WindowsForms;
using NetTopologySuite.Geometries;
using WaterRoute.Infrastructure;

namespace WaterRoute.Controls;

public sealed class RouteMapControl : MapControl
{
    private readonly MapService _mapService;
    private readonly string _mbTilesPath;

    public RouteMapControl()
    {
        _mbTilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps", "saint-petersburg.mbtiles");
        _mapService = new MapService();
        Dock = DockStyle.Fill;
        BackColor = System.Drawing.Color.FromArgb(232, 241, 238);
    }

    public void SetRoute(double warehouseLon, double warehouseLat, double deliveryLon, double deliveryLat, string routeNumber, string status)
    {
        _mapService.SetRoute(
            new NetTopologySuite.Geometries.Point(warehouseLon, warehouseLat),
            new NetTopologySuite.Geometries.Point(deliveryLon, deliveryLat),
            routeNumber);

        var centerLon = (warehouseLon + deliveryLon) / 2;
        var centerLat = (warehouseLat + deliveryLat) / 2;
        _mapService.SetCenter(centerLat, centerLon, 13);
    }

    public void ClearRoute() => _mapService.Clear();
    public MapService Service => _mapService;
}