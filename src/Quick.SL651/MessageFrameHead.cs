using Quick.SL651.Utils;

namespace Quick.SL651
{
    public class MessageFrameHead : TelemetryStationInfo
    {
        /// <summary>
        /// ASCII编码帧起始符
        /// </summary>
        public readonly static byte[] ASCII_S0H = { 0x30, 0x31 };
        /// <summary>
        /// HEX/BCD编码帧起始符
        /// </summary>
        public readonly static byte[] HEX_BCD_S0H = { 0x7E, 0x7E };
        /// <summary>
        /// 报文起始符
        /// </summary>
        public readonly static byte STX = 0x02;
        /// <summary>
        /// 报文结束，后续无报文
        /// </summary>
        public readonly static byte ETX = 0x03;
        /// <summary>
        /// 报文结束，后续有报文
        /// </summary>
        public readonly static byte ETB = 0x17;

        /// <summary>
        /// 帧编码
        /// </summary>
        public FrameEncoding FrameEncoding { get; set; }
        /// <summary>
        /// 功能码
        /// </summary>
        public byte FunctionCode { get; set; }
        /// <summary>
        /// 是否上行
        /// </summary>
        public bool IsUpgoing { get; set; }
        /// <summary>
        /// 报文长度
        /// </summary>
        public int MessageLength { get; set; }

        //读取包头
        private async Task<int> ReadPackageHead(Stream stream, byte[] read_buffer, int bufferStartIndex, CancellationToken cancellationToken, int readTimeout)
        {
            //读取包头
            bufferStartIndex += await TransportUtils.ReadData(stream, read_buffer, bufferStartIndex, 2, cancellationToken, readTimeout);
            //如果是HEX/BCD编码
            if (read_buffer.StartWith(MessageFrameHead.HEX_BCD_S0H))
            {
                FrameEncoding = FrameEncoding.HEX_BCD;
            }
            else if (read_buffer.StartWith(MessageFrameHead.ASCII_S0H))
            {
                FrameEncoding = FrameEncoding.ASCII;
            }
            else
            {
                throw new IOException("未知包头：" + BitConverter.ToString(read_buffer, bufferStartIndex - 2, 2));
            }
            return bufferStartIndex;
        }

        //读取中心站地址
        private async Task<int> ReadCentralStationAddress(Stream stream, byte[] read_buffer, int bufferStartIndex, CancellationToken cancellationToken, int readTimeout)
        {
            bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 1, cancellationToken, readTimeout);
            CentralStationAddress = read_buffer[bufferStartIndex - 1];
            return bufferStartIndex;
        }

        //读取遥测站地址
        private async Task<int> ReadTelemetryStationAddress(Stream stream, byte[] read_buffer, int bufferStartIndex, CancellationToken cancellationToken, int readTimeout)
        {
            bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 5, cancellationToken, readTimeout);
            TelemetryStationAddress = new Span<byte>(read_buffer, bufferStartIndex - 5, 5).BCD_Decode();
            return bufferStartIndex;
        }

        //读取密码
        private async Task<int> ReadPassword(Stream stream, byte[] read_buffer, int bufferStartIndex, CancellationToken cancellationToken, int readTimeout)
        {
            bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 2, cancellationToken, readTimeout);
            Password = new byte[2];
            Password[0] = read_buffer[bufferStartIndex - 2];
            Password[1] = read_buffer[bufferStartIndex - 1];
            return bufferStartIndex;
        }

        //读取功能码
        private async Task<int> ReadFunctionCode(Stream stream, byte[] read_buffer, int bufferStartIndex, CancellationToken cancellationToken, int readTimeout)
        {
            bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 1, cancellationToken, readTimeout);
            FunctionCode = read_buffer[bufferStartIndex - 1];
            return bufferStartIndex;
        }

        //读取报文上下行标识和报文长度
        private async Task<int> ReadIsUpgoingAndMessageLength(Stream stream, byte[] read_buffer, int bufferStartIndex, CancellationToken cancellationToken, int readTimeout)
        {
            bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 2, cancellationToken, readTimeout);
            read_buffer[bufferStartIndex] = read_buffer[bufferStartIndex - 2];
            read_buffer[bufferStartIndex] &= 0xF0;
            var isUpgoing = read_buffer[bufferStartIndex] == 0;
            if (IsUpgoing != isUpgoing)
            {
                if (IsUpgoing)
                    throw new IOException("预期接收上行报文，却收到了下行报文");
                else
                    throw new IOException("预期接收下行报文，却收到了上行报文");
            }
            read_buffer[bufferStartIndex] = read_buffer[bufferStartIndex - 2];
            read_buffer[bufferStartIndex + 1] = read_buffer[bufferStartIndex - 1];
            read_buffer[bufferStartIndex] &= 0x0F;
            //如果CPU是小端字节序，则交换
            if (BitConverter.IsLittleEndian)
            {
                var tmpByte = read_buffer[bufferStartIndex];
                read_buffer[bufferStartIndex] = read_buffer[bufferStartIndex + 1];
                read_buffer[bufferStartIndex + 1] = tmpByte;
            }
            MessageLength = BitConverter.ToInt16(read_buffer, bufferStartIndex);
            read_buffer[bufferStartIndex] = 0;
            read_buffer[bufferStartIndex + 1] = 0;
            return bufferStartIndex;
        }

        public async Task<int> Read(
            bool isUpgoing,
            Stream stream,
            byte[] read_buffer,
            int bufferStartIndex,
            CancellationToken cancellationToken,
            int readTimeout)
        {
            IsUpgoing = isUpgoing;
            //读取包头
            bufferStartIndex = await ReadPackageHead(stream, read_buffer, bufferStartIndex, cancellationToken, readTimeout);
            //如果是上行报文
            if (IsUpgoing)
            {
                //读取中心站地址
                bufferStartIndex = await ReadCentralStationAddress(stream, read_buffer, bufferStartIndex, cancellationToken, readTimeout);
                //读取遥测站地址
                bufferStartIndex = await ReadTelemetryStationAddress(stream, read_buffer, bufferStartIndex, cancellationToken, readTimeout);
            }
            //否则是下行报文
            else
            {
                //读取遥测站地址
                bufferStartIndex = await ReadTelemetryStationAddress(stream, read_buffer, bufferStartIndex, cancellationToken, readTimeout);
                //读取中心站地址
                bufferStartIndex = await ReadCentralStationAddress(stream, read_buffer, bufferStartIndex, cancellationToken, readTimeout);
            }
            //读取密码
            bufferStartIndex = await ReadPassword(stream, read_buffer, bufferStartIndex, cancellationToken, readTimeout);
            //读取功能码
            bufferStartIndex = await ReadFunctionCode(stream, read_buffer, bufferStartIndex, cancellationToken, readTimeout);
            //读取报文上下行标识和报文长度
            bufferStartIndex = await ReadIsUpgoingAndMessageLength(stream, read_buffer, bufferStartIndex, cancellationToken, readTimeout);
            return bufferStartIndex;
        }
    }
}
