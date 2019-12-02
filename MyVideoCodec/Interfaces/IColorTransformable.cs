using System;
using System.Collections.Generic;
using System.Text;

namespace MyVideoCodec.Interfaces
{
    public interface IColorTransformable
    {
        IEnumerable<Rgb> ConvertYuvToRgb(IEnumerable<Yuv> yuvBytes);
        IEnumerable<Yuv> ConvertRgbToYuv(IEnumerable<Rgb> rgbBytes);
    }
}
