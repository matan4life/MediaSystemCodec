using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Riff
{
    public static class ParserOperations
    {
        public static int GetInt32FromByteSequence(IEnumerable<byte> bytes)
        {
            if (bytes.Count() != 4)
            {
                throw new ArgumentException("Incorrect byte sequence!");
            }
            return BitConverter.ToInt32(bytes.ToArray(), 0);
        }

        public static uint GetUInt32FromByteSequence(IEnumerable<byte> bytes)
        {
            if (bytes.Count() != 4)
            {
                throw new ArgumentException("Incorrect byte sequence!");
            }
            return BitConverter.ToUInt32(bytes.ToArray(), 0);
        }

        public static short GetInt16FromByteSequence(IEnumerable<byte> bytes)
        {
            if (bytes.Count() != 2)
            {
                throw new ArgumentException("Incorrect byte sequence!");
            }
            return BitConverter.ToInt16(bytes.ToArray(), 0);
        }

        public static ushort GetUInt16FromByteSequence(IEnumerable<byte> bytes)
        {
            if (bytes.Count() != 2)
            {
                throw new ArgumentException("Incorrect byte sequence!");
            }
            return BitConverter.ToUInt16(bytes.ToArray(), 0);
        }

        public static string GetFourCCFromBytes(IEnumerable<byte> bytes)
        {
            if (bytes.Count() != 4)
            {
                throw new ArgumentException("Incorrect byte sequence!");
            }

            uint fourCCCode = GetUInt32FromByteSequence(bytes);
            return new string(
                    new[]
                    {
                        (char)(fourCCCode & 0xFF),
                        (char)((fourCCCode & 0xFF00) >> 8),
                        (char)((fourCCCode & 0xFF0000) >> 16),
                        (char)((fourCCCode & 0xFF000000U) >> 24)
                    }
                );
        }
        public static List<T> GetFlagsList<T>(uint value) where T : Enum
        {
            return ((T[])Enum.GetValues(typeof(T))).Where(x => DoesFlagContain(value, Convert.ToUInt32(x))).ToList();
        }

        public static bool DoesFlagContain(uint value, uint flag)
        {
            return ((value & flag) == flag);
        }

    }
}
