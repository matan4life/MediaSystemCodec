using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Riff.ParserOperations;

namespace Riff.Avi
{
    public struct AviMainHeader
    {
        public AviMainHeader(uint microSecondsPerFrame, uint maxBytesPerSecond, uint paddingGranularity, List<AviMainHeaderFlags> headerFlags, uint totalFrames, uint initialFrames, uint streamsCount, uint suggestedBufferSize, uint screenWidth, uint screenHeight, uint[] reservedBuffer = null)
        {
            MicroSecondsPerFrame = microSecondsPerFrame;
            MaxBytesPerSecond = maxBytesPerSecond;
            PaddingGranularity = paddingGranularity;
            HeaderFlags = headerFlags;
            TotalFrames = totalFrames;
            InitialFrames = initialFrames;
            StreamsCount = streamsCount;
            SuggestedBufferSize = suggestedBufferSize;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            ReservedBuffer = reservedBuffer ?? new uint[0];
        }

        public uint MicroSecondsPerFrame { get; private set; }
        public uint MaxBytesPerSecond { get; private set; }
        public uint PaddingGranularity { get; private set; }
        public List<AviMainHeaderFlags> HeaderFlags { get; private set; }
        public uint TotalFrames { get; private set; }
        public uint InitialFrames { get; private set; }
        public uint StreamsCount { get; private set; }
        public uint SuggestedBufferSize { get; private set; }
        public uint ScreenWidth { get; private set; }
        public uint ScreenHeight { get; private set; }
        public uint[] ReservedBuffer { get; private set; }

        public static AviMainHeader GetAviMainHeader(IEnumerable<byte> bytes)
        {
            var list = bytes as List<byte> ?? new List<byte>(bytes);
            return new AviMainHeader(
                    GetUInt32FromByteSequence(list.GetRange(0, 4)),
                    GetUInt32FromByteSequence(list.GetRange(4, 4)),
                    GetUInt32FromByteSequence(list.GetRange(8, 4)),
                    GetFlagsList<AviMainHeaderFlags>(GetUInt32FromByteSequence(list.GetRange(12, 4))),
                    GetUInt32FromByteSequence(list.GetRange(16, 4)),
                    GetUInt32FromByteSequence(list.GetRange(20, 4)),
                    GetUInt32FromByteSequence(list.GetRange(24, 4)),
                    GetUInt32FromByteSequence(list.GetRange(28, 4)),
                    GetUInt32FromByteSequence(list.GetRange(32, 4)),
                    GetUInt32FromByteSequence(list.GetRange(36, 4)),
                    new uint[]
                    {
                        GetUInt32FromByteSequence(list.GetRange(40, 4)),
                        GetUInt32FromByteSequence(list.GetRange(44, 4)),
                        GetUInt32FromByteSequence(list.GetRange(48, 4)),
                        GetUInt32FromByteSequence(list.GetRange(52, 4))
                    }
                );
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var value in this.GetType().GetProperties())
            {
                switch (value.GetValue(this))
                {
                    case uint number:
                        stringBuilder.AppendLine($"{value.Name}: {number}");
                        break;
                    case List<AviMainHeaderFlags> flags when flags.Any():
                        stringBuilder.Append($"{value.Name}: ");
                        flags.ForEach(x => stringBuilder.Append($"{x.ToString()}; "));
                        stringBuilder.Append("\n");
                        break;
                    default:
                        break;
                }
            }
            return stringBuilder.ToString();
        }
    }
}
