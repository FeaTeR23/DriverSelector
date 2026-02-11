using System.Collections.Generic;
namespace DriverLocator.Models;
public interface INearestDriverFinder
{
    List<(int driverId, double distance)> FindNearest(
        DriverRegistry registry, 
        int orderX, 
        int orderY, 
        int count = 5);
}