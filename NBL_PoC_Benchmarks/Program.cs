// See https://aka.ms/new-console-template for more information


using BenchmarkDotNet.Running;
using NBL_PoC_DatabaseBenchmark;

BenchmarkRunner.Run<DatabaseBenchmark>();