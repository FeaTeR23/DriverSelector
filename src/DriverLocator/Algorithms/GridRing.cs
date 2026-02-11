using System;
using System.Collections.Generic;
using DriverLocator.Models;

namespace DriverLocator.Algorithms;

public class GridRingFinder : INearestDriverFinder
{
    public List<(int driverId, double distance)> FindNearest(
        DriverRegistry registry, int orderX, int orderY, int count = 5)
    {
        var results = new List<(int driverId, double distance)>();
        var visited = new HashSet<(int x, int y)>();
        var cellMap = registry.GetCellMapping();
        int radius = 0;

        // Расширяем кольца, пока не наберём нужное количество водителей
        while (results.Count < count && radius <= Math.Max(registry.Width, registry.Height))
        {
            // Все клетки с манхэттенским расстоянием == radius
            for (int dx = -radius; dx <= radius; dx++)
            {
                int absDx = Math.Abs(dx);
                int dy1 = radius - absDx;
                int dy2 = -dy1;

                CheckCell(orderX + dx, orderY + dy1);
                if (dy2 != dy1) CheckCell(orderX + dx, orderY + dy2);
            }
            radius++;
        }

        results.Sort((a, b) => a.distance.CompareTo(b.distance));
        return results;

        void CheckCell(int x, int y)
        {
            if (x < 0 || x >= registry.Width || y < 0 || y >= registry.Height) 
                return;
            
            var key = (x, y);
            if (visited.Contains(key) || !cellMap.TryGetValue(key, out int driverId)) 
                return;

            visited.Add(key);
            double dx = x - orderX;
            double dy = y - orderY;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            results.Add((driverId, dist));
        }
    }
}