using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using System.Reflection;

namespace RDFSharp.Benchmarks
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args, DefaultConfig.Instance
                .WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(100)));
        }
    }
}