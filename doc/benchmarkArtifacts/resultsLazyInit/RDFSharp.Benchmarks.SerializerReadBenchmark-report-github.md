``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.928 (2004/?/20H1)
Intel Core i5-7600K CPU 3.80GHz (Kaby Lake), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=5.0.201
  [Host]     : .NET Core 5.0.4 (CoreCLR 5.0.421.11614, CoreFX 5.0.421.11614), X64 RyuJIT
  DefaultJob : .NET Core 5.0.4 (CoreCLR 5.0.421.11614, CoreFX 5.0.421.11614), X64 RyuJIT


```
| Method |                               InputFile |     Mean |   Error |  StdDev |
|------- |---------------------------------------- |---------:|--------:|--------:|
|   **Read** | **(Samples/dbpediaNTriples.zip, NTriples)** | **402.8 ms** | **7.65 ms** | **8.19 ms** |
|   **Read** |     **(Samples/dbpediaRdfXml.zip, RdfXml)** | **287.7 ms** | **2.99 ms** | **2.80 ms** |
|   **Read** |         **(Samples/dbpediaTriX.zip, TriX)** | **308.0 ms** | **3.79 ms** | **3.54 ms** |
|   **Read** |     **(Samples/dbpediaTurtle.zip, Turtle)** | **419.1 ms** | **5.65 ms** | **5.01 ms** |
