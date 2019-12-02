using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace MyVideoCodec
{
    public class Rle
    {
        public IEnumerable<sbyte> Compress(IEnumerable<sbyte> bytes)
        {
            var list = bytes.ToList();
            sbyte zeroCounter = 0;
            for (int i=0; i<list.Count; i++)
            {
                if (list[i] != 0 || (i==list.Count-1 && list.Last() == 0))
                {
                    yield return zeroCounter;
                    yield return list[i];
                    zeroCounter = 0;
                }
                else zeroCounter++;
            }
        }
        public IEnumerable<sbyte> Decompress(IEnumerable<sbyte> bytes)
        {
            Contract.Requires(bytes.Count() % 2 == 0);
            var list = bytes.ToList();
            for (int i=0; i < list.Count/2; i++)
            {
                var sequence = list.Skip(2 * i).Take(2);
                foreach (var zero in Enumerable.Repeat((sbyte)0, sequence.First()))
                {
                    yield return zero;
                }
                yield return sequence.Last();
            }
        }
    }
}
