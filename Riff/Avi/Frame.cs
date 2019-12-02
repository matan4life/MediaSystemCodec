using System;
using System.Collections.Generic;
using System.Text;

namespace Riff.Avi
{
    public struct Frame
    {
        public Frame(short left, short top, short right, short bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public short Left { get; private set; }
        public short Top { get; private set; }
        public short Right { get; private set; }
        public short Bottom { get; private set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Frame parameters: ");
            foreach (var property in this.GetType().GetProperties())
            {
                stringBuilder.AppendLine($"{property.Name}: {property.GetValue(this)}px");
            }
            return stringBuilder.ToString();
        }
    }
}
