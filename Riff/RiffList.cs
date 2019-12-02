using Riff.Avi;
using System;
using System.Collections.Generic;
using System.Linq;
using static Riff.ParserOperations;

namespace Riff
{
    public class RiffList : RiffElement
    {
        public RiffList(int startPosition, RiffList parent) : base(startPosition, parent) { }

        public IEnumerable<RiffElement> Children { get; private set; }

        public override void Parse(IEnumerable<byte> bytes)
        {
            var list = bytes as List<byte> ?? new List<byte>(bytes);
            this.Id = (this.Parent is null) ? "RIFF" : "LIST";
            this.Size = GetInt32FromByteSequence(list.GetRange(4, 4));
            if ((this.Size & 1) != 0) this.Size++;
            this.DataType = GetFourCCFromBytes(list.GetRange(8, 4));
            this.Children = GetChildren(list.GetRange(12, this.Size - 4));
        }

        private IEnumerable<RiffElement> GetChildren(List<byte> bytes)
        {
            var result = new List<RiffElement>();
            for (int i = 0; i < bytes.Count;)
            {
                int elementSize = GetInt32FromByteSequence(bytes.GetRange(i + 4, 4)) + 8;
                if ((elementSize & 1) != 0) elementSize++;
                RiffElement element;
                int startPosition = this.StartPosition + 12;
                if (GetFourCCFromBytes(bytes.GetRange(i, 4)) == "LIST")
                {
                    element = new RiffList(startPosition + i, this);
                }
                else
                {
                    var id = GetFourCCFromBytes(bytes.GetRange(i, 4));
                    switch (id)
                    {
                        case "avih":
                            element = new RiffChunk<AviMainHeader>(startPosition + i, this);
                            break;
                        case "strh":
                            element = new RiffChunk<AviStreamHeader>(startPosition + i, this);
                            break;
                        default:
                            element = new RiffChunk<byte>(startPosition + i, this);
                            break;
                    }
                }
                element.Parse(bytes.GetRange(i, elementSize));
                result.Add(element);
                i += elementSize;
            }
            return result;
        }
    }
}
