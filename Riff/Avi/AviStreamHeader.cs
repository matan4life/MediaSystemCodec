using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Riff.ParserOperations;

namespace Riff.Avi
{
    public struct AviStreamHeader
    {
        public AviStreamHeader(string type, string handler, List<AviStreamHeaderFlags> flags, ushort priority, ushort language, uint frames, uint scale, uint rate, uint start, uint length, uint suggestedBufferSize, uint quality, uint sampleSize, Frame frame)
        {
            Type = type;
            Handler = handler;
            Flags = flags;
            Priority = priority;
            Language = language;
            Frames = frames;
            Scale = scale;
            Rate = rate;
            Start = start;
            Length = length;
            SuggestedBufferSize = suggestedBufferSize;
            Quality = quality;
            SampleSize = sampleSize;
            Frame = frame;
        }

        public string Type { get; private set; }
        public string Handler { get; private set; }
        public List<AviStreamHeaderFlags> Flags { get; private set; }
        public ushort Priority { get; private set; }
        public ushort Language { get; private set; }
        public uint Frames { get; private set; }
        public uint Scale { get; private set; }
        public uint Rate { get; private set; }
        public uint Start { get; private set; }
        public uint Length { get; private set; }
        public uint SuggestedBufferSize { get; private set; }
        public uint Quality { get; private set; }
        public uint SampleSize { get; private set; }
        public Frame Frame { get; private set; }

        public static AviStreamHeader GetAviStreamHeader(List<byte> bytes)
        {
            return new AviStreamHeader(
                    GetFourCCFromBytes(bytes.GetRange(0, 4)),
                    GetFourCCFromBytes(bytes.GetRange(4, 4)),
                    GetFlagsList<AviStreamHeaderFlags>(GetUInt32FromByteSequence(bytes.GetRange(8, 4))),
                    GetUInt16FromByteSequence(bytes.GetRange(12, 2)),
                    GetUInt16FromByteSequence(bytes.GetRange(14, 2)),
                    GetUInt32FromByteSequence(bytes.GetRange(16, 4)),
                    GetUInt32FromByteSequence(bytes.GetRange(20, 4)),
                    GetUInt32FromByteSequence(bytes.GetRange(24, 4)),
                    GetUInt32FromByteSequence(bytes.GetRange(28, 4)),
                    GetUInt32FromByteSequence(bytes.GetRange(32, 4)),
                    GetUInt32FromByteSequence(bytes.GetRange(36, 4)),
                    GetUInt32FromByteSequence(bytes.GetRange(40, 4)),
                    GetUInt32FromByteSequence(bytes.GetRange(44, 4)),
                    new Frame(
                            GetInt16FromByteSequence(bytes.GetRange(48, 2)),
                            GetInt16FromByteSequence(bytes.GetRange(50, 2)),
                            GetInt16FromByteSequence(bytes.GetRange(52, 2)),
                            GetInt16FromByteSequence(bytes.GetRange(54, 2))
                        )
                );
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var value in this.GetType().GetProperties())
            {
                switch (value.GetValue(this))
                {
                    case string stringValue:
                    case int intVariable:
                    case uint uintVariable:
                        stringBuilder.AppendLine($"{value.Name}: {value.GetValue(this)}");
                        break;
                    case Frame frame:
                        stringBuilder.Append(frame.ToString());
                        break;
                    case List<AviStreamHeaderFlags> flags when flags.Any():
                        stringBuilder.Append("Flags: ");
                        flags.ForEach(x => stringBuilder.Append(x.ToString()));
                        stringBuilder.Append("\n");
                        break;
                }
            }
            return stringBuilder.ToString();
        }
    }
}
