using System;
using System.Collections.Generic;
using System.Text;

namespace MyVideoCodec
{
    public interface ICompressable
    {
        IEnumerable<byte> Compress(IEnumerable<byte> decompressed);
    }
}
