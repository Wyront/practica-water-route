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

    public RouteMapControl()
    {
        _mapService = new MapService();
        Map = _mapService.Map;
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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _mapService.Dispose();
        base.Dispose(disposing);
    }
}