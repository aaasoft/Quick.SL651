using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quick.SL651.Enums;

namespace Quick.SL651.Utils
{
    public class TransportUtils
    {
        public static async Task<int> ReadData(
            FrameEncoding frameEncoding,
            Stream stream,
            byte[] buffer,
            int startIndex,
            int totalCount,
            CancellationToken cancellationToken,
            int readTimeout)
        {
            var sourceTotalCount = totalCount;
            //如果是ASCII编码，读取的字节数翻倍
            if (frameEncoding == FrameEncoding.ASCII)
                totalCount *= 2;
            await ReadData(stream, buffer, startIndex, totalCount, cancellationToken, readTimeout);
            //将ASCII编码转换为HEX/BCD编码
            if (frameEncoding == FrameEncoding.ASCII)
                for (var i = 0; i < totalCount; i += 2)
                {
                    var str = Encoding.ASCII.GetString(buffer, startIndex + i, 2);
                    var b = byte.Parse(str, System.Globalization.NumberStyles.HexNumber);
                    buffer[i / 2 + startIndex] = b;
                }
            return sourceTotalCount;
        }

        public static async Task<int> ReadData(
            Stream stream,
            byte[] buffer,
            int startIndex,
            int totalCount,
            CancellationToken cancellationToken,
            int readTimeout)
        {
            if (totalCount > buffer.Length - startIndex)
                throw new IOException($"Recv data length[{totalCount}] bigger than buffer length[{buffer.Length - startIndex}]");
            int ret;
            var count = 0;
            while (count < totalCount)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                var readTask = stream.ReadAsync(buffer, count + startIndex, totalCount - count, cancellationToken);
                ret = await await TaskUtils.TaskWait(readTask, readTimeout);
                if (readTask.IsCanceled || ret == 0)
                    break;
                if (ret < 0)
                    throw new IOException("Read error from stream.");
                count += ret;
            }
            return count;
        }
    }
}
