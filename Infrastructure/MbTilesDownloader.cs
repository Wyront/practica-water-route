using System.Text.Json;
using Mapsui.Tiling;
using BruTile.Predefined;

namespace WaterRoute.Infrastructure;

public sealed class MbTilesDownloader
{
    /// <summary>
    /// Скачивает тайлы Санкт-Петербурга в MBTiles файл.
    /// Требует подготовки файла saint-petersburg.mbtiles отдельно.
    /// </summary>
    public static async Task DownloadSaintPetersburgAsync(string outputPath, IProgress<(int current, int total, string status)>? progress = null, CancellationToken ct = default)
    {
        // Заглушка: создаем пустой MBTiles файл с метаданными
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={outputPath}");
        await connection.OpenAsync(ct);

        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS metadata (name TEXT PRIMARY KEY, value TEXT);
                CREATE TABLE IF NOT EXISTS tiles (zoom_level INTEGER, tile_column INTEGER, tile_row INTEGER, tile_data BLOB, 
                    PRIMARY KEY (zoom_level, tile_column, tile_row));
                CREATE INDEX IF NOT EXISTS tile_index ON tiles (zoom_level, tile_column, tile_row);";
            await cmd.ExecuteNonQueryAsync(ct);
        }

        // Записываем метаданные
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "INSERT OR REPLACE INTO metadata (name, value) VALUES (@name, @value)";
            var nameParam = cmd.CreateParameter(); nameParam.ParameterName = "@name";
            var valueParam = cmd.CreateParameter(); valueParam.ParameterName = "@value";
            cmd.Parameters.Add(nameParam);
            cmd.Parameters.Add(valueParam);

            foreach (var (name, value) in new[]
            {
                ("name", "Saint Petersburg Offline"),
                ("type", "baselayer"),
                ("version", "1.0"),
                ("description", "Offline map of Saint Petersburg for WaterRoute - prepare manually"),
                ("format", "png"),
                ("bounds", "30.0,59.7,30.6,60.1"),
                ("minzoom", "10"),
                ("maxzoom", "15"),
                ("center", "30.3351,59.9343,12")
            })
            {
                nameParam.Value = name;
                valueParam.Value = value;
                await cmd.ExecuteNonQueryAsync(ct);
            }
        }

        progress?.Report((0, 0, "MBTiles file created with metadata only. Download tiles manually using external tools."));
        await Task.CompletedTask;
    }
}