using System.Collections.Generic;
namespace DriverLocator.Models;
public class DriverRegistry
{
    private readonly int N; // ширина по сетке
    private readonly int M; // высота по сетке
    private readonly Dictionary<int, (int x, int y)> _drivers = new();
    private readonly Dictionary<(int x, int y), int> _cellToDriver = new();

    public DriverRegistry(int width, int height)
    {
        N = width;
        M = height;
    }

    public void AddOrUpdateDriver(int driverId, int x, int y)
    {
        if (x < 0 || x >= N || y < 0 || y >= M)
            throw new System.ArgumentOutOfRangeException(nameof(x), "Координаты вне границ карты");

        // Удаляем старую позицию
        if (_drivers.TryGetValue(driverId, out var oldPos))
            _cellToDriver.Remove((oldPos.x, oldPos.y));

        _drivers[driverId] = (x, y);
        _cellToDriver[(x, y)] = driverId;
    }

    public IReadOnlyDictionary<int, (int x, int y)> GetAllDrivers() => _drivers;
    public IReadOnlyDictionary<(int x, int y), int> GetCellMapping() => _cellToDriver;
    public int Width => N;
    public int Height => M;
}