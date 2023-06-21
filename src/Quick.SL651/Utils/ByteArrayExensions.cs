using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ByteArrayExensions
    {
        public static bool StartWith(this byte[] buffer, ReadOnlySpan<byte> value)
        {
            return StartWith(buffer, 0, value);
        }

        public static bool StartWith(this byte[] buffer, int startIndex, ReadOnlySpan<byte> value)
        {
            if (buffer.Length - startIndex < value.Length)
                return false;
            for (var i = 0; i < value.Length; i++)
            {
                if (buffer[startIndex + i] != value[i])
                    return false;
            }
            return true;
        }
    }
}
