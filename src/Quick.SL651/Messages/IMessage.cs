using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    /// <summary>
    /// 报文
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// 流水号
        /// </summary>
        ushort SerialNumber { get; }
        /// <summary>
        /// 发报时间
        /// </summary>
        DateTime SendTime { get; }

        /// <summary>
        /// 从字节Span中读取
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        Span<byte> Read(Span<byte> span);

        /// <summary>
        /// 写入到流
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        int Write(Stream stream);
    }
}
