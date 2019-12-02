using Riff.Avi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Riff.ParserOperations;

namespace Riff
{
    public class RiffChunk<T> : RiffElement where T : struct
    {
        public RiffChunk(int startPosition, RiffList parent) : base(startPosition, parent) { }

        public IEnumerable<T> Children { get; private set; }

        public override void Parse(IEnumerable<byte> bytes)
        {
            var list = bytes as List<byte> ?? new List<byte>(bytes);
            this.Id = GetFourCCFromBytes(list.GetRange(0, 4));
            this.Size = GetInt32FromByteSequence(list.GetRange(4, 4));
            if ((this.Size & 1) != 0) this.Size++;
            this.DataType = typeof(T).Name;
            this.Children = GetChildren(list.GetRange(8, this.Size));
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder(base.ToString());
            var t = new T();
            switch (t)
            {
                case object value when value is AviMainHeader || value is AviStreamHeader:
                    return stringBuilder.Append(this.Children.First().ToString()).ToString();
                default:
                    return stringBuilder.ToString();
            }
        }

        private IEnumerable<T> GetChildren(List<byte> bytes)
        {
            var t = new T();
            return t switch
            {
                byte _ => bytes.Cast<T>(),
                AviMainHeader _ => new List<AviMainHeader>() { AviMainHeader.GetAviMainHeader(bytes) }.Cast<T>(),
                AviStreamHeader _ => new List<AviStreamHeader>() { AviStreamHeader.GetAviStreamHeader(bytes) }.Cast<T>(),
                _ => null,
            };
        }
    }
}
