
using System.Text;

namespace System
{
    public static class SpanExtensions
    {
        public static string BCD_Decode(this Span<byte> span)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in span)
            {
                sb.Append(b.ToString("X2"));
            }
            var str = sb.ToString();
            sb.Clear();
            return str;
        }
    }
}
