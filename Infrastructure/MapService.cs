using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using NetTopologySuite.Geometries;
using WaterRoute.Models;
using Brush = Mapsui.Styles.Brush;

namespace WaterRoute.Infrastructure;

public sealed class MapService : IDisposable
{
    private MemoryLayer _routeLayer = null!;
    private MemoryLayer _markersLayer = null!;
    private readonly Map _map;
    private bool _disposed;

    public MapService()
    {
        _map = new Map();
        // Базовый слой: OpenStreetMap (онлайн).
        _map.Layers.Add(OpenStreetMap.CreateTileLayer());

        _routeLayer = new MemoryLayer { Name = "Route" };
        _markersLayer = new MemoryLayer { Name = "Markers" };
        _map.Layers.Add(_routeLayer);
        _map.Layers.Add(_markersLayer);
    }

    public Map Map => _map;

    public void SetCenter(double latitude, double longitude, int zoom = 12)
    {
        var center = SphericalMercator.FromLonLat(longitude, latitude);
        _map.Navigator.CenterOn(center.x, center.y);
        _map.Navigator.ZoomTo(zoom);
    }

    public void SetRoute(NetTopologySuite.Geometries.Point warehouse, NetTopologySuite.Geometries.Point destination, string routeName)
    {
        ResetLayers();

        var wMerc = SphericalMercator.FromLonLat(warehouse.X, warehouse.Y);
        var dMerc = SphericalMercator.FromLonLat(destination.X, destination.Y);

        // Маркеры (GeometryFeature с NTS Point в EPSG:3857)
        var whFeature = new GeometryFeature(new NetTopologySuite.Geometries.Point(wMerc.x, wMerc.y))
        {
            ["Name"] = "Склад",
            ["Type"] = "warehouse"
        };
        whFeature.Styles.Add(new SymbolStyle
        {
            SymbolType = SymbolType.Ellipse,
            Fill = new Brush(ToMapsuiColor(UITheme.Primary)),
            Outline = new Mapsui.Styles.Pen(ToMapsuiColor(UITheme.Surface), 3)
        });
        _markersLayer.Features = _markersLayer.Features.Append(whFeature);

        var destFeature = new GeometryFeature(new NetTopologySuite.Geometries.Point(dMerc.x, dMerc.y))
        {
            ["Name"] = "Доставка",
            ["Type"] = "destination"
        };
        destFeature.Styles.Add(new SymbolStyle
        {
            SymbolType = SymbolType.Ellipse,
            Fill = new Mapsui.Styles.Brush(ToMapsuiColor(UITheme.Danger)),
            Outline = new Mapsui.Styles.Pen(ToMapsuiColor(UITheme.Surface), 3)
        });
        _markersLayer.Features = _markersLayer.Features.Append(destFeature);

        // Линия маршрута
        var lineString = new LineString(new[]
        {
            new Coordinate(wMerc.x, wMerc.y),
            new Coordinate(dMerc.x, dMerc.y)
        });
        var routeFeature = new GeometryFeature(lineString) { ["Name"] = routeName };
        routeFeature.Styles.Add(new VectorStyle
        {
            Line = new Mapsui.Styles.Pen(ToMapsuiColor(UITheme.Primary), 3)
        });
        _routeLayer.Features = _routeLayer.Features.Append(routeFeature);

        // Центрируем по обеим точкам (EPSG:3857)
        var rect = new MRect(
            Math.Min(wMerc.x, dMerc.x),
            Math.Min(wMerc.y, dMerc.y),
            Math.Max(wMerc.x, dMerc.x),
            Math.Max(wMerc.y, dMerc.y));
        _map.Navigator.ZoomToBox(rect);
    }

    public void Clear() => ResetLayers();

    private void ResetLayers()
    {
        _routeLayer.Features = Enumerable.Empty<IFeature>();
        _markersLayer.Features = Enumerable.Empty<IFeature>();
    }

    private static Mapsui.Styles.Color ToMapsuiColor(System.Drawing.Color color) =>
        new(color.R, color.G, color.B, color.A);

    public void Dispose()
    {
        if (!_disposed)
        {
            _map.Layers.Clear();
            _disposed = true;
        }
    }
}