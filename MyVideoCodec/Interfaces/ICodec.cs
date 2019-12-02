using System;
using System.Collections.Generic;
using System.Text;

namespace MyVideoCodec
{
    public interface ICodec
    {
        IEnumerable<byte> Compress(IEnumerable<byte> decompressed);
        IEnumerable<byte> Decompress(IEnumerable<byte> compressed);
    }
}
