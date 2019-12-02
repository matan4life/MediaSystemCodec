using System;
using System.Collections.Generic;
using System.Text;

namespace MyVideoCodec.Interfaces
{
    public interface IQuantizable
    {
        IEnumerable<sbyte> Quantize(IEnumerable<sbyte> input);
        IEnumerable<sbyte> Dequantize(IEnumerable<sbyte> input);
    }
}
