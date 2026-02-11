using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using DriverLocator;
using DriverLocator.Algorithms;
using System;
using DriverLocator.Models;

[MemoryDiagnoser]
[RankColumn]
public class FinderBenchmark
{
    private DriverRegistry _registry;
    private int _orderX, _orderY;
    private readonly Random _rnd = new(42);

    [Params(100, 1_000, 10_000)]
    public int DriverCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        const int width = 1000, height = 1000;
        _registry = new DriverRegistry(width, height);
        
        // Генерируем водителей (разрешаем коллизии для простоты бенчмарка)
        for (int i = 0; i < DriverCount; i++)
        {
            int x = _rnd.Next(width);
            int y = _rnd.Next(height);
            _registry.AddOrUpdateDriver(i, x, y);
        }
        
        _orderX = _rnd.Next(width);
        _orderY = _rnd.Next(height);
    }

    [Benchmark]
    public object LinearScan() => new LinearScanFinder()
        .FindNearest(_registry, _orderX, _orderY);

    [Benchmark]
    public object GridRing() => new GridRingFinder()
        .FindNearest(_registry, _orderX, _orderY);

    [Benchmark]
    public object KDTree() => new KDTreeFinder()
        .FindNearest(_registry, _orderX, _orderY);
}