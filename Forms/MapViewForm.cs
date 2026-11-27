using Mapsui.UI.WindowsForms;
using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed partial class MapViewForm : BaseEditForm
{
    private MapControl _mapControl = null!;
    private MapService? _mapService;

    public MapViewForm() : base("Карта доставки", "Санкт-Петербург · онлайн-карта OpenStreetMap · без GPS и дорожной оптимизации", 1200, 800)
    {
        InitializeComponent();
        TopLevel = false;
        FormBorderStyle = FormBorderStyle.None;
        ControlBox = false;
        Load += MapViewForm_Load;
    }

    private void MapViewForm_Load(object? sender, EventArgs e)
    {
        try
        {
            _mapService = new MapService();
            _mapControl.Map = _mapService.Map;
            _mapService.SetCenter(59.9343, 30.3351, 11);

            var warehouse = new NetTopologySuite.Geometries.Point(30.3351, 59.9343);
            var destination = new NetTopologySuite.Geometries.Point(30.3601, 59.9289);
            _mapService.SetRoute(warehouse, destination, "R-021");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка инициализации карты: {ex.Message}", "WaterRoute", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    protected override void Dispose(bool disposing)
    {
        _mapService?.Dispose();
        base.Dispose(disposing);
    }
}