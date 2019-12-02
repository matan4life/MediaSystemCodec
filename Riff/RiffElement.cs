using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Riff
{
    public abstract class RiffElement
    {
        protected RiffElement(int startPosition, RiffList parent)
        {
            this.StartPosition = startPosition;
            this.Parent = parent;
        }

        public int StartPosition { get; protected set; }
        public string Id { get; protected set; }
        public int Size { get; protected set; }
        public string DataType { get; protected set; }
        public RiffList Parent { get; protected set; }

        public abstract void Parse(IEnumerable<byte> bytes);

        public override string ToString()
        {
            return $"Id: {this.Id} Type: {this.DataType} {this.StartPosition}...{this.StartPosition + this.Size}";
        }
    }
}
