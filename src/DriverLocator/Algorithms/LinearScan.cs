using DriverLocator.Models;

namespace DriverLocator.Algorithms;
public class LinearScanFinder : INearestDriverFinder
{
    public List<(int driverId, double distance)> FindNearest(
        DriverRegistry registry, int orderX, int orderY, int count = 5)
    {
        var drivers = registry.GetAllDrivers();
        if (drivers.Count == 0) 
            return new List<(int, double)>();

        // Собираем всех водителей с расстояниями
        var list = drivers.Select(d => (
            driverId: d.Key,
            distance: Math.Sqrt(Math.Pow(d.Value.x - orderX, 2) + Math.Pow(d.Value.y - orderY, 2))
        )).ToList();

        // Сортируем: сначала по расстоянию, потом по ID (для стабильности)
        list.Sort((a, b) =>
        {
            int distCmp = a.distance.CompareTo(b.distance);
            return distCmp != 0 ? distCmp : a.driverId.CompareTo(b.driverId);
        });

        return list.Take(count).ToList();
    }
}