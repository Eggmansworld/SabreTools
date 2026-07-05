using System;
using System.IO;

namespace SabreTools.FileTypes
{
    /// <summary>
    /// Read-only wrapper that limits reading to a fixed number of bytes from an
    /// underlying stream. Prevents decompression streams that support concatenated
    /// frames (e.g. Zstandard) from reading past an archive entry's data into the
    /// following entry headers. Disposing the wrapper does not dispose the
    /// underlying stream.
    /// </summary>
    internal class ReadLimitedStream : Stream
    {
        private readonly Stream _baseStream;
        private long _remaining;

        public ReadLimitedStream(Stream baseStream, long length)
        {
            _baseStream = baseStream;
            _remaining = length;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_remaining <= 0)
                return 0;

            if (count > _remaining)
                count = (int)_remaining;

            int read = _baseStream.Read(buffer, offset, count);
            _remaining -= read;
            return read;
        }

        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
