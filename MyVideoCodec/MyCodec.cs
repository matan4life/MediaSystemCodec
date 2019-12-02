using LinearAlgebraModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyVideoCodec
{
    public class MyCodec : ICodec
    {
        private ICompressable videoCompressor { get; }
        private IDecompressable videoDecompressor { get; }
        public IEnumerable<byte> Compress(IEnumerable<byte> decompressed)
        {
            return this.videoCompressor.Compress(decompressed);
        }

        public IEnumerable<byte> Decompress(IEnumerable<byte> compressed)
        {
            return this.videoDecompressor.Decompress(compressed);
        }
    }
}
