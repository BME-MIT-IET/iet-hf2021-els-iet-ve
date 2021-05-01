using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RDFSharp.Benchmarks.Streams
{
#pragma warning disable S3881 // "IDisposable" should be implemented correctly
    internal abstract class StreamProxy : Stream
#pragma warning restore S3881 // "IDisposable" should be implemented correctly
    {
        protected readonly Stream inner;

        public override bool CanRead => inner.CanRead;

        public override bool CanSeek => inner.CanSeek;

        public override bool CanWrite => inner.CanWrite;

        public override bool CanTimeout => inner.CanTimeout;

        public override long Length => inner.Length;

        public override long Position { get => inner.Position; set => inner.Position = value; }

        public override int ReadTimeout { get => inner.ReadTimeout; set => inner.ReadTimeout = value; }

        public override int WriteTimeout { get => inner.WriteTimeout; set => inner.WriteTimeout = value; }

        protected StreamProxy(Stream inner)
        {
            this.inner = inner;
        }

        public override void Flush() => inner.Flush();

        public override Task FlushAsync(CancellationToken cancellationToken) => inner.FlushAsync(cancellationToken);

        public override long Seek(long offset, SeekOrigin origin) => inner.Seek(offset, origin);

        public override void SetLength(long value) => inner.SetLength(value);

        public override int ReadByte() => inner.ReadByte();

        public override int Read(byte[] buffer, int offset, int count) => inner.Read(buffer, offset, count);

        public override int Read(Span<byte> buffer) => inner.Read(buffer);

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => inner.ReadAsync(buffer, offset, count, cancellationToken);

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => inner.ReadAsync(buffer, cancellationToken);

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => inner.BeginRead(buffer, offset, count, callback, state);

        public override int EndRead(IAsyncResult asyncResult) => inner.EndRead(asyncResult);

        public override void WriteByte(byte value) => inner.WriteByte(value);

        public override void Write(byte[] buffer, int offset, int count) => inner.Write(buffer, offset, count);

        public override void Write(ReadOnlySpan<byte> buffer) => inner.Write(buffer);

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => inner.WriteAsync(buffer, offset, count, cancellationToken);

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) => inner.WriteAsync(buffer, cancellationToken);

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => inner.BeginWrite(buffer, offset, count, callback, state);

        public override void EndWrite(IAsyncResult asyncResult) => inner.EndWrite(asyncResult);

        public override void CopyTo(Stream destination, int bufferSize) => inner.CopyTo(destination, bufferSize);

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) => inner.CopyToAsync(destination, bufferSize, cancellationToken);

        [Obsolete("Base member is obsolete.")]
        public override object InitializeLifetimeService() => inner.InitializeLifetimeService();

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    inner.Dispose();
                }

                disposedValue = true;
            }
        }
        #endregion
    }
}
