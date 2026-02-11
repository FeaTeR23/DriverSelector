using System;
using System.Collections.Generic;
using System.Linq;
using DriverLocator.Models;

namespace DriverLocator.Algorithms;

// Узел KD-дерева
class KDNode
{
    public int DriverId { get; }
    public int X { get; }
    public int Y { get; }
    public KDNode Left { get; set; }
    public KDNode Right { get; set; }
    public int Axis { get; } // 0 = X, 1 = Y

    public KDNode(int driverId, int x, int y, int axis)
    {
        DriverId = driverId;
        X = x;
        Y = y;
        Axis = axis;
    }
}

public class KDTreeFinder : INearestDriverFinder
{
    private KDNode _root;
    private DriverRegistry _lastRegistry;
    private int _lastHash;

    public List<(int driverId, double distance)> FindNearest(
        DriverRegistry registry, int orderX, int orderY, int count = 5)
    {
        // Кэшируем дерево, если реестр не изменился
        int hash = registry.GetAllDrivers().Count;
        if (_root == null || registry != _lastRegistry || hash != _lastHash)
        {
            _root = BuildTree(registry.GetAllDrivers().Select(kv => 
                (kv.Key, kv.Value.x, kv.Value.y)).ToList(), 0);
            _lastRegistry = registry;
            _lastHash = hash;
        }

        var results = new SortedSet<(double dist, int id)>(
            Comparer<(double dist, int id)>.Create((a, b) =>
                a.dist != b.dist ? b.dist.CompareTo(a.dist) : a.id.CompareTo(b.id)));

        SearchNearest(_root, orderX, orderY, count, results);
        return results.Reverse().Select(x => (x.id, x.dist)).ToList();
    }

    private KDNode BuildTree(List<(int id, int x, int y)> points, int depth)
    {
        if (points.Count == 0) return null;

        int axis = depth % 2; // 0 = X, 1 = Y
        points.Sort((a, b) => axis == 0 ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y));
        int mid = points.Count / 2;

        var node = new KDNode(points[mid].id, points[mid].x, points[mid].y, axis);
        node.Left = BuildTree(points.Take(mid).ToList(), depth + 1);
        node.Right = BuildTree(points.Skip(mid + 1).ToList(), depth + 1);
        return node;
    }

    private void SearchNearest(KDNode node, int qx, int qy, int k, 
        SortedSet<(double dist, int id)> results)
    {
        if (node == null) return;

        double dx = node.X - qx;
        double dy = node.Y - qy;
        double dist = Math.Sqrt(dx * dx + dy * dy);
        results.Add((dist, node.DriverId));
        if (results.Count > k) results.Remove(results.Max);

        // Определяем, в какую ветку идти первой
        bool goLeft = (node.Axis == 0) ? qx < node.X : qy < node.Y;
        var first = goLeft ? node.Left : node.Right;
        var second = goLeft ? node.Right : node.Left;

        SearchNearest(first, qx, qy, k, results);

        // Проверяем необходимость обхода второй ветки
        double bestDist = results.Count > 0 ? results.Max.dist : double.MaxValue;
        double axisDist = node.Axis == 0 ? Math.Abs(qx - node.X) : Math.Abs(qy - node.Y);
        if (axisDist < bestDist)
            SearchNearest(second, qx, qy, k, results);
    }
}