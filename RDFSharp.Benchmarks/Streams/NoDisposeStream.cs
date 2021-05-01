using System.IO;
using System.Threading.Tasks;

namespace RDFSharp.Benchmarks.Streams
{
#pragma warning disable S3881 // "IDisposable" should be implemented correctly
    internal class NoDisposeStream : StreamProxy
#pragma warning restore S3881 // "IDisposable" should be implemented correctly
    {
        public NoDisposeStream(Stream inner) : base(inner)
        {
        }

        protected override void Dispose(bool disposing)
        {
            // Do nothing.
        }

        public override ValueTask DisposeAsync() => default;
    }
}
