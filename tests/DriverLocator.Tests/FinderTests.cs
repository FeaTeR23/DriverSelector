using NUnit.Framework;
using DriverLocator;
using DriverLocator.Algorithms;
using System.Linq;
using DriverLocator.Models;

namespace DriverLocator.Tests;

[TestFixture]
public class FinderTests
{
    [Test]
    public void AllAlgorithms_ReturnSameDriversForSmallGrid()
    {
        // Arrange
        var registry = new DriverRegistry(10, 10);
        registry.AddOrUpdateDriver(1, 0, 0);
        registry.AddOrUpdateDriver(2, 1, 1);
        registry.AddOrUpdateDriver(3, 2, 2);
        registry.AddOrUpdateDriver(4, 3, 3);
        registry.AddOrUpdateDriver(5, 4, 4);
        registry.AddOrUpdateDriver(6, 9, 9);

        var orderX = 0; 
        var orderY = 0;
        var expectedIds = new[] { 1, 2, 3, 4, 5 };

        var finder = new LinearScanFinder();
        var result = finder.FindNearest(registry, orderX, orderY, 5);
        var actualIds = result.Select(r => r.driverId).ToArray();

        // Assert — ПРАВИЛЬНЫЙ СИНТАКСИС ДЛЯ NUNIT 4+
        Assert.That(result.Count, Is.EqualTo(5));
        Assert.That(actualIds, Is.EqualTo(expectedIds));
    }

    [Test]
    public void HandlesLessThanFiveDrivers()
    {
        var smallRegistry = new DriverRegistry(5, 5);
        smallRegistry.AddOrUpdateDriver(10, 1, 1);
        smallRegistry.AddOrUpdateDriver(20, 2, 2);

        var result = new LinearScanFinder().FindNearest(smallRegistry, 0, 0, 5);
        
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].driverId, Is.EqualTo(10));
    }

    [Test]
    public void DistanceCalculation_IsAccurate()
    {
        var registry = new DriverRegistry(100, 100);
        registry.AddOrUpdateDriver(100, 3, 4); // Расстояние до (0,0) = 5.0

        var result = new LinearScanFinder().FindNearest(registry, 0, 0, 1);
        
        Assert.That(result[0].distance, Is.EqualTo(5.0).Within(1e-6));
    }
}