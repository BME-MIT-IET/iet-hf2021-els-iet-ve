using BenchmarkDotNet.Attributes;
using RDFSharp.Benchmarks.Streams;
using RDFSharp.Model;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace RDFSharp.Benchmarks
{
    [RPlotExporter]
    public class SerializerReadBenchmark
    {
        [ParamsSource(nameof(InputFiles))]
        public (string path, RDFModelEnums.RDFFormats format) InputFile { get; set; }

        public (string path, RDFModelEnums.RDFFormats format)[] InputFiles => new[] {
            ("Samples/dbpediaNTriples.zip", RDFModelEnums.RDFFormats.NTriples),
            ("Samples/dbpediaRdfXml.zip", RDFModelEnums.RDFFormats.RdfXml),
            ("Samples/dbpediaTriX.zip", RDFModelEnums.RDFFormats.TriX),
            ("Samples/dbpediaTurtle.zip", RDFModelEnums.RDFFormats.Turtle)
        };

        private MemoryStream inputData;
        private NoDisposeStream inputDataNoDispose;

        [GlobalSetup]
        public void GlobalSetup()
        {
            using FileStream fs = new FileStream(InputFile.path, FileMode.Open, FileAccess.Read);
            using ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Read, true);
            using Stream entry = archive.Entries.First().Open();

            inputData = new MemoryStream();
            entry.CopyTo(inputData);
            inputDataNoDispose = new NoDisposeStream(inputData);
        }

        [Benchmark]
        public void Read()
        {
            inputData.Seek(0, SeekOrigin.Begin);

#pragma warning disable S1481, IDE0059 // Unused local variables should be removed, Unnecessary assignment of a value
            RDFGraph graph = RDFGraph.FromStream(InputFile.format, inputDataNoDispose);
#pragma warning restore S1481, IDE0059
        }

        [GlobalCleanup]
        public void GlobalCreanup()
        {
            inputData.Dispose();
        }
    }
}