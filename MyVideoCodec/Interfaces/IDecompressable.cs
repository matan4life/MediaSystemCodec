using System;
using System.Collections.Generic;
using System.Text;

namespace MyVideoCodec
{
    public interface IDecompressable
    {
        IEnumerable<byte> Decompress(IEnumerable<byte> compressed);
    }
}
