using LinearAlgebraModule;
using MyVideoCodec.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyVideoCodec
{
    public class MyVideoDecompresser : IDecompressable
    {
        public MyVideoDecompresser(
                IColorTransformable colorTransformer,
                IDctTransformable dctTransformer,
                IQuantizable quantizer,
                IDictionary<sbyte, decimal> entropyTable)
        {
            this.ColorTransformer = colorTransformer;
            this.DctTransformer = dctTransformer;
            this.Quantizer = quantizer;
            this.RleCodec = new Rle();
            this.ArithmeticalCodec = new ArithmeticalCodec(entropyTable);
        }
        private IColorTransformable ColorTransformer { get; }
        private IDctTransformable DctTransformer { get; }
        private IQuantizable Quantizer { get; }
        private Rle RleCodec { get; }
        private ArithmeticalCodec ArithmeticalCodec { get; }
        public IEnumerable<byte> Decompress(IEnumerable<byte> compressed)
        {
            var bits = new BitArray(compressed.ToArray());
            var tmpArray = new int[bits.Count];
            bits.CopyTo(tmpArray, 0);
            decimal encoded = new decimal(tmpArray);
            var mixedBytes = this.ArithmeticalCodec.Decompress(encoded, 6);
            var yCounter = mixedBytes.Count() / 6;
            IEnumerable<byte> Ys = mixedBytes.Take(4 * yCounter).Select(x => (byte)(x + 128));
            IEnumerable<byte> Us = this.DctTransformer.ConvertReversely(
                                        this.Quantizer.Dequantize(
                                                this.RleCodec.Decompress(
                                                        mixedBytes.Skip(4 * yCounter).Take(yCounter)
                                                    )
                                            )
                );
            IEnumerable<byte> Vs = this.DctTransformer.ConvertReversely(
                            this.Quantizer.Dequantize(
                                    this.RleCodec.Decompress(
                                            mixedBytes.Skip(5 * yCounter).Take(yCounter)
                                        )
                                )
                );

            var indices = Enumerable.Range(0, 64).Select(x => x * 4).ToList();
            var VlagrangeTable = indices.Zip(Vs, (x, y) => new { x, y }).ToDictionary(x => (double)x.x, x => (double)x.y);
            var UlagrangeTable = indices.Zip(Us, (x, y) => new { x, y }).ToDictionary(x => (double)x.x, x => (double)x.y);
            List<Yuv> almostDecompressed = new List<Yuv>();
            for (int i=0; i<256; i++)
            {
                if (indices.Contains(i))
                {
                    almostDecompressed.Add(new Yuv()
                    {
                        Y = Ys.ToList()[i],
                        U = Us.ToList()[i],
                        V = Vs.ToList()[i]
                    });
                }
                else
                {
                    almostDecompressed.Add(new Yuv()
                    {
                        Y = Ys.ToList()[i],
                        U = (byte)LagrangePolynomial.CalculateLagrangePolynomialValue(i, UlagrangeTable),
                        V = (byte)LagrangePolynomial.CalculateLagrangePolynomialValue(i, VlagrangeTable)
                    });
                }
            }
            var rgb = this.ColorTransformer.ConvertYuvToRgb(almostDecompressed);
            foreach (var value in rgb)
            {
                yield return value.R;
                yield return value.G;
                yield return value.B;
            }
        }
    }
}
