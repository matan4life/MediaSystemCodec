using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyVideoCodec
{
    public class ArithmeticalCodec
    {
        public ArithmeticalCodec(IDictionary<sbyte, decimal> entropyTable)
        {
            this.EntropyTable = entropyTable;
        }
        private IDictionary<sbyte, decimal> EntropyTable { get; set; }

        public decimal Compress(IEnumerable<sbyte> block, int maxPrecision)
        {
            var workLine = this.BuildWorkLine();
            for (int i=0; i<block.Count(); i++)
            {
                sbyte element = block.Skip(i).Take(1).First();
                workLine = this.RebuildWorkLine(workLine, element);
            }
            var lastTuple = workLine[block.Last()];
            return lastTuple.start;
        }

        public IEnumerable<sbyte> Decompress(decimal encoded, int maxPrecision)
        {
            var workLine = this.BuildWorkLine();
            while (true)
            {
                var value = Math.Round(encoded, maxPrecision, MidpointRounding.AwayFromZero);
                var element = workLine.TakeWhile(x => x.Value.start <= value).Last();
                if (value == element.Value.start)
                {
                    break;
                }
                yield return element.Key;
                encoded = (encoded - element.Value.start) / (element.Value.end - element.Value.start);
            }
        }

        private IDictionary<sbyte, (decimal start, decimal end)> BuildWorkLine()
        {
            var result = new Dictionary<sbyte, (decimal start, decimal end)>();
            var orderedProbability = this.EntropyTable.OrderByDescending(x => x.Value).ThenBy(x => x.Key).ToDictionary(x=>x.Key, x=>x.Value);
            decimal marker = 0M;
            foreach (var pair in orderedProbability)
            {
                result.Add(pair.Key, (marker, pair.Value+marker));
                marker += pair.Value;
            }
            return result;
        }

        private IDictionary<sbyte, (decimal start, decimal end)> RebuildWorkLine(IDictionary<sbyte, (decimal start, decimal end)> workLine, sbyte currentSymbol)
        {
            var oldTuple = workLine[currentSymbol];
            var result = new Dictionary<sbyte, (decimal start, decimal end)>();
            var orderedProbability = this.EntropyTable.OrderByDescending(x => x.Value).ThenBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            var length = oldTuple.end - oldTuple.start;
            var marker = oldTuple.start;
            foreach (var element in orderedProbability)
            {
                result.Add(element.Key, (marker, marker + length * element.Value));
                marker += length * element.Value;
            }
            return result;
        }
    }
}
