using LinearAlgebraModule;
using MyVideoCodec.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace MyVideoCodec
{
    public class Quantizer : IQuantizable
    {
        private static int QuantizingLevel { get; } = 2;
        private static Matrix QuantumMatrix
        {
            get
            {
                List<List<double>> result = new List<List<double>>();
                Enumerable.Range(0, 8).ToList().ForEach(x =>
                {
                    var row = new List<double>();
                    Enumerable.Range(0, 8).ToList().ForEach(y =>
                    {
                        row.Add(1 + ((1 + x + y) * QuantizingLevel));
                    });
                    result.Add(row);
                });
                return new Matrix(result);
            }
        }
        public IEnumerable<sbyte> Dequantize(IEnumerable<sbyte> input)
        {
            Contract.Requires(input.Count() == 64);
            return input.Zip(QuantumMatrix.Rows.SelectMany(x => x).Select(x => (sbyte)x), (x, y) => (sbyte)(x * y));
        }

        public IEnumerable<sbyte> Quantize(IEnumerable<sbyte> input)
        {
            Contract.Requires(input.Count() == 64);
            return input.Zip(QuantumMatrix.Rows.SelectMany(x => x).Select(x => (sbyte)x), (x, y) => (sbyte)(x / y));
        }
    }
}
