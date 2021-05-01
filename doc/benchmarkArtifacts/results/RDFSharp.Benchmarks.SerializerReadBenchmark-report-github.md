``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.928 (2004/?/20H1)
Intel Core i5-7600K CPU 3.80GHz (Kaby Lake), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=5.0.201
  [Host]     : .NET Core 5.0.4 (CoreCLR 5.0.421.11614, CoreFX 5.0.421.11614), X64 RyuJIT
  DefaultJob : .NET Core 5.0.4 (CoreCLR 5.0.421.11614, CoreFX 5.0.421.11614), X64 RyuJIT


```
| Method |                               InputFile |     Mean |   Error |   StdDev |
|------- |---------------------------------------- |---------:|--------:|---------:|
|   **Read** | **(Samples/dbpediaNTriples.zip, NTriples)** | **434.9 ms** | **8.49 ms** | **11.62 ms** |
|   **Read** |     **(Samples/dbpediaRdfXml.zip, RdfXml)** | **312.6 ms** | **6.17 ms** | **12.03 ms** |
|   **Read** |         **(Samples/dbpediaTriX.zip, TriX)** | **340.9 ms** | **6.66 ms** |  **6.84 ms** |
|   **Read** |     **(Samples/dbpediaTurtle.zip, Turtle)** | **457.1 ms** | **8.74 ms** |  **8.18 ms** |
