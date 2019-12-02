using LinearAlgebraModule;
using MyVideoCodec.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace MyVideoCodec
{
    public class DctTransformer : IDctTransformable
    {
        private static Matrix DctMatrix
        {
            get
            {
                var result = new List<List<double>>() {
                    Enumerable.Range(0, 8).Select(x=>1 / Math.Sqrt(8)).ToList()
                };
                for (int i = 1; i < 8; i++)
                {
                    var row = new List<double>();
                    for (int j = 0; j < 8; j++)
                    {
                        row.Add(0.5 * Math.Cos(((2 * j + 1) * i * Math.PI) / 16));
                    }
                    result.Add(row);
                }
                return new Matrix(result);
            }
        }
        public IEnumerable<sbyte> Convert(IEnumerable<byte> input)
        {
            Contract.Requires(input.Count() == 64);
            var list = new List<List<double>>();
            Enumerable.Range(0, 8)
                .ToList()
                .ForEach(x => list.Add(input.Skip(x * 8).Take(8).Select(x=>(double)(x)).ToList()));
            var matrix = new Matrix(list) - new Matrix(Enumerable.Range(0, 8).Select(x => Enumerable.Repeat(128.0, 8).ToList()).ToList());
            var tmp = matrix * DctMatrix.Transpose();
            var result = tmp * DctMatrix;
            return result.Rows.SelectMany(x => x).Select(x => (sbyte)x);
        }

        public IEnumerable<byte> ConvertReversely(IEnumerable<sbyte> input)
        {
            Contract.Requires(input.Count() == 64);
            var list = new List<List<double>>();
            Enumerable.Range(0, 8)
                .ToList()
                .ForEach(x => list.Add(input.Skip(x * 8).Take(8).Select(x => (double)(x)).ToList()));
            var tmp = new Matrix(list) * Matrix.GetReverseMatrix(DctMatrix);
            var result = (tmp * Matrix.GetReverseMatrix(DctMatrix.Transpose())) + new Matrix(Enumerable.Range(0, 8).Select(x => Enumerable.Repeat(128.0, 8).ToList()).ToList());
            return result.Rows.SelectMany(x => x).Select(x => (byte)x);
        }
    }
}
