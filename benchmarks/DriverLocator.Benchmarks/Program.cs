using BenchmarkDotNet.Running;

namespace DriverLocator.Benchmarks;

class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<FinderBenchmark>();
    }
}