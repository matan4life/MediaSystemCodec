using LinearAlgebraModule;
using MyVideoCodec.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyVideoCodec
{
    public class MyVideoCompresser : ICompressable
    {
        public MyVideoCompresser(IColorTransformable colorTransformer, IDctTransformable dctTransformer, IQuantizable quantizer)
        {
            ColorTransformer = colorTransformer;
            DctTransformer = dctTransformer;
            Quantizer = quantizer;
            this.RleCodec = new Rle();
        }

        private IColorTransformable ColorTransformer { get; }
        private IDctTransformable DctTransformer { get; }
        private IQuantizable Quantizer { get; }
        private Rle RleCodec { get; }
        private ArithmeticalCodec ArithmeticalCodec { get; set; }
        private IDictionary<sbyte, decimal> EntropyTable { get; set; } = new Dictionary<sbyte, decimal>();

        public IEnumerable<byte> Compress(IEnumerable<byte> decompressed)
        {
            List<Rgb> rgbs = new List<Rgb>();
            for (int i=0; i<=decompressed.Count()-3; i += 3)
            {
                rgbs.Add(new Rgb()
                {
                    R = decompressed.ToList()[i],
                    G = decompressed.ToList()[i + 1],
                    B = decompressed.ToList()[i + 2]
                });
            }
            List<Yuv> yuvs = this.ColorTransformer.ConvertRgbToYuv(rgbs).ToList();
            List<sbyte> Ys = yuvs.Select(x => (sbyte)(x.Y - 128)).ToList();
            List<byte> Us = yuvs.Select(x => x.U).Where((x, i) => i % 4 == 0).ToList();
            List<byte> Vs = yuvs.Select(x=>x.V).Where((x, i) => i % 4 == 0).ToList();
            var UTransformed = this.RleCodec.Compress(Matrix.GetBypass(this.Quantizer.Quantize(this.DctTransformer.Convert(Us)), 8));
            var VTransformed = this.RleCodec.Compress(Matrix.GetBypass(this.Quantizer.Quantize(this.DctTransformer.Convert(Vs)), 8));
            if (!this.EntropyTable.Any())
            {
                this.EntropyTable = GetEntropy(Ys.Concat(UTransformed).Concat(VTransformed));
            }
            this.ArithmeticalCodec = new ArithmeticalCodec(this.EntropyTable);
            decimal result = this.ArithmeticalCodec.Compress(Ys.Concat(UTransformed).Concat(VTransformed), 6);
            var bits = new BitArray(decimal.GetBits(result));
            var bytes = new byte[bits.Count / 8];
            bits.CopyTo(bytes, 0);
            return bytes;
        }

        public static IDictionary<sbyte, decimal> GetFullEntropyTablePerFile(IEnumerable<sbyte> bytes)
        {
            var result = GetEntropy(bytes);
            for (sbyte value = sbyte.MinValue; value<=sbyte.MaxValue; value++)
            {
                if (result.ContainsKey(value)) break;
                result.Add(value, 0.0M);
            }
            return result.OrderBy(x => x.Key).ThenBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        private static IDictionary<sbyte, decimal> GetEntropy(IEnumerable<sbyte> bytes)
        {
            return bytes.GroupBy(x => x).Select(x => new KeyValuePair<sbyte, decimal>(x.Key, ((decimal)bytes.Count(y => y == x.Key)) / bytes.Count())).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
