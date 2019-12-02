using System;
using System.Collections.Generic;
using System.Text;

namespace MyVideoCodec.Interfaces
{
    public interface IDctTransformable
    {
        IEnumerable<sbyte> Convert(IEnumerable<byte> input);
        IEnumerable<byte> ConvertReversely(IEnumerable<sbyte> input);
    }
}
