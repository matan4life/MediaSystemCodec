using LinearAlgebraModule;
using MyVideoCodec.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyVideoCodec
{
    public class ColorTransformer : IColorTransformable
    {
        private static List<List<double>> RgbToYuvCoefficients { get; } = new List<List<double>>()
        {
            new List<double>()
            {
                0.299,
                -0.168736,
                0.5
            },
            new List<double>()
            {
                0.587,
                -0.331264,
                -0.418688
            },
            new List<double>()
            {
                0.114,
                0.5,
                -0.081312
            }
        };

        private static List<List<double>> YuvToRgbCoefficients { get; } = new List<List<double>>()
        {
            new List<double>()
            {
                1,
                1,
                1
            },
            new List<double>()
            {
                0,
                -0.34414,
                1.772
            },
            new List<double>()
            {
                1.402,
                -0.71414,
                0
            }
        };
        public IEnumerable<Yuv> ConvertRgbToYuv(IEnumerable<Rgb> rgbBytes)
        {
            return rgbBytes.Select(x =>
            {
                var rgb = new List<double>()
                {
                    x.R,
                    x.G,
                    x.B
                };
                Matrix matrix = new Matrix(RgbToYuvCoefficients.Zip(rgb, (x, y) => x.Select(z => z * y).ToList()).ToList()).Transpose();
                return new Yuv()
                {
                    Y = (byte)matrix.Rows[0].Sum(),
                    U = (byte)(matrix.Rows[1].Sum() + 128),
                    V = (byte)(matrix.Rows[2].Sum() + 128)
                };
            });
        }

        public IEnumerable<Rgb> ConvertYuvToRgb(IEnumerable<Yuv> yuvBytes)
        {
            return yuvBytes.Select(x =>
            {
                var yuv = new List<double>()
                {
                    x.Y,
                    x.U - 128,
                    x.V - 128
                };
                Matrix matrix = new Matrix(YuvToRgbCoefficients.Zip(yuv, (x, y) => x.Select(z => z * y).ToList()).ToList()).Transpose();
                return new Rgb()
                {
                    R = (byte)matrix.Rows[0].Sum(),
                    G = (byte)matrix.Rows[1].Sum(),
                    B = (byte)matrix.Rows[2].Sum()
                };
            });
        }
    }
}
