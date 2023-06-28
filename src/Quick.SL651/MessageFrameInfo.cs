using Quick.SL651.Enums;
using Quick.SL651.Utils;
using System.IO.Hashing;
using System.Net.Sockets;

namespace Quick.SL651
{
    /// <summary>
    /// 消息帧头部
    /// </summary>
    public class MessageFrameInfo : TelemetryStationInfo
    {
        /// <summary>
        /// ASCII编码帧起始符
        /// </summary>
        public readonly static byte[] ASCII_S0H = { 0x30, 0x31 };
        /// <summary>
        /// HEX/BCD编码帧起始符
        /// </summary>
        public readonly static byte[] HEX_BCD_S0H = { 0x7E, 0x7E };

        public MessageFrameInfo(WorkMode workMode, bool isUpgoing)
        {
            WorkMode = workMode;
            IsUpgoing = isUpgoing;
        }

        /// <summary>
        /// 工作模式
        /// </summary>
        public WorkMode WorkMode { get; set; }
        /// <summary>
        /// 功能码
        /// </summary>
        public FunctionCodes FunctionCode { get; set; }
        /// <summary>
        /// 是否上行
        /// </summary>
        public bool IsUpgoing { get; set; }
        /// <summary>
        /// 报文长度
        /// </summary>
        public int MessageLength { get; set; }
        /// <summary>
        /// 包总数
        /// </summary>
        public ushort PackageCount { get; set; }
        /// <summary>
        /// 包序列号
        /// </summary>
        public ushort PackageIndex { get; set; }
        /// <summary>
        /// 报文开始符
        /// </summary>
        public StartMarks StartMark { get; set; }
        /// <summary>
        /// 报文结束符
        /// </summary>
        public EndMarks EndMark { get; set; }
        
        //读取包头
        private async Task<int> ReadPackageHead(Stream stream, byte[] read_buffer, int bufferStartIndex, CancellationToken cancellationToken, int readTimeout)
        {
            //读取包头
            bufferStartIndex += await TransportUtils.ReadData(stream, read_buffer, bufferStartIndex, 2, cancellationToken, readTimeout);
            //如果是HEX/BCD编码
            if (read_buffer.StartWith(HEX_BCD_S0H))
            {
                FrameEncoding = FrameEncoding.HEX_BCD;
            }
            else if (read_buffer.StartWith(ASCII_S0H))
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
            TelemetryStationAddress = new Span<byte>(read_buffer, bufferStartIndex - 5, 5).ToArray();
            TelemetryStationAddress_Text = new Span<byte>(TelemetryStationAddress).BCD_Decode();
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
            FunctionCode = (FunctionCodes)read_buffer[bufferStartIndex - 1];
            return bufferStartIndex;
        }

        //读取报文上下行标识和报文长度
        private async Task<int> ReadIsUpgoingAndMessageLength(Stream stream, byte[] read_buffer, int bufferStartIndex, CancellationToken cancellationToken, int readTimeout)
        {
            bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 2, cancellationToken, readTimeout);
            var b0 = read_buffer[bufferStartIndex - 2];
            var b1 = read_buffer[bufferStartIndex - 1];
            //解析是否上行
            b0 &= 0xF0;
            var isUpgoing = b0 == 0;
            if (IsUpgoing != isUpgoing)
            {
                if (IsUpgoing)
                    throw new IOException("预期接收上行报文，却收到了下行报文");
                else
                    throw new IOException("预期接收下行报文，却收到了上行报文");
            }
            //解析报文长度
            b0 = read_buffer[bufferStartIndex - 2];
            b0 &= 0x0F;
            //如果CPU是小端字节序，则交换位置解析
            if (BitConverter.IsLittleEndian)
                MessageLength = BitConverter.ToInt16(new byte[] { b1, b0 });
            else
                MessageLength = BitConverter.ToInt16(new byte[] { b0, b1 });
            return bufferStartIndex;
        }

        //读取报文开始符
        private async Task<int> ReadMessageStartMark(Stream stream, byte[] read_buffer, int bufferStartIndex, CancellationToken cancellationToken, int readTimeout)
        {
            bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 1, cancellationToken, readTimeout);
            StartMark = (StartMarks)read_buffer[bufferStartIndex - 1];
            switch (WorkMode)
            {
                case WorkMode.M1:
                case WorkMode.M2:
                case WorkMode.M4:
                    if (StartMark != StartMarks.STX)
                        throw new IOException($"意外的字符：0x{read_buffer[bufferStartIndex - 1].ToString("X2")}。预期字符：报文开始符(传输正文起始:{StartMarks.STX})");
                    break;
                case WorkMode.M3:
                    if (StartMark != StartMarks.SYN)
                        throw new IOException($"意外的字符：0x{read_buffer[bufferStartIndex - 1].ToString("X2")}。预期字符：报文开始符(多包传输正文起始:{StartMarks.SYN})");
                    break;
            }
            return bufferStartIndex;
        }

        //读取包总数和序列号
        private async Task<int> ReadPackageCountAndPackageIndex(Stream stream, byte[] read_buffer, int bufferStartIndex, CancellationToken cancellationToken, int readTimeout)
        {
            bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 3, cancellationToken, readTimeout);
            //解析包总数
            var b0 = read_buffer[bufferStartIndex - 2];
            var b1 = read_buffer[bufferStartIndex - 1];
            b1 &= 0xF0;

            //如果是小端字节序，则交换位置解析
            if (BitConverter.IsLittleEndian)
                PackageCount = BitConverter.ToUInt16(new byte[] { b1, b0 });
            else
                PackageCount = BitConverter.ToUInt16(new byte[] { b0, b1 });
            PackageCount = Convert.ToUInt16(PackageCount >> 4);
            //解析包序列号
            b0 = read_buffer[bufferStartIndex - 2];
            b1 = read_buffer[bufferStartIndex - 1];
            b0 &= 0x0F;
            //如果C是小端字节序，则交换位置解析
            if (BitConverter.IsLittleEndian)
                PackageIndex = BitConverter.ToUInt16(new byte[] { b1, b0 });
            else
                PackageIndex = BitConverter.ToUInt16(new byte[] { b0, b1 });

            return bufferStartIndex;
        }

        public async Task<int> ReadHead(
            Stream stream,
            byte[] read_buffer,
            int bufferStartIndex,
            CancellationToken cancellationToken,
            int readTimeout)
        {
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
            //读取报文开始符
            bufferStartIndex = await ReadMessageStartMark(stream, read_buffer, bufferStartIndex, cancellationToken, readTimeout);
            //如果是多包传输
            if (StartMark == StartMarks.SYN)
            {
                //读取包总数及序列号
                bufferStartIndex = await ReadPackageCountAndPackageIndex(stream, read_buffer, bufferStartIndex, cancellationToken, readTimeout);
            }
            return bufferStartIndex;
        }

        //读取报文结束符
        private async Task<int> ReadEndMark(Stream stream, byte[] read_buffer, int bufferStartIndex, CancellationToken cancellationToken, int readTimeout)
        {
            bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 1, cancellationToken, readTimeout);
            CentralStationAddress = read_buffer[bufferStartIndex - 1];
            return bufferStartIndex;
        }

        //验证CRC校验值
        private async Task<int> ValidateCRC(Stream stream, byte[] read_buffer, int bufferStartIndex, CancellationToken cancellationToken, int readTimeout)
        {
            //计算CRC
            var crcResult = Crc16.Hash(new ReadOnlySpan<byte>(read_buffer, 0, bufferStartIndex));
            //读取CRC
            bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 2, cancellationToken, readTimeout);
            //比较
            if (read_buffer[bufferStartIndex - 2] != crcResult[0]
                || read_buffer[bufferStartIndex - 1] != crcResult[1])
                throw new IOException("报文帧CRC检验失败！");
            return bufferStartIndex;
        }

        public async Task<int> ReadTail(NetworkStream stream, byte[] read_buffer, int bufferStartIndex, CancellationToken cancellationToken, int readTimeout)
        {
            //读取报文结束符
            bufferStartIndex = await ReadEndMark(stream, read_buffer, bufferStartIndex, cancellationToken, readTimeout);
            //报文结束符
            EndMark = (EndMarks)read_buffer[bufferStartIndex - 1];
            if (IsUpgoing)
            {
                if (EndMark != EndMarks.ETB
                    && EndMark != EndMarks.ETX)
                    throw new IOException($"意外的字符：0x{EndMark.ToString("X2")}。预期字符：报文结束符({EndMarks.ETX}或者{EndMarks.ETB})");
            }
            else
            {
                switch (WorkMode)
                {
                    case WorkMode.M1:
                        throw new IOException("M1工作模式没有下行帧");
                    case WorkMode.M2:
                    case WorkMode.M4:
                        switch(EndMark)
                        {
                            case EndMarks.ENQ:
                            case EndMarks.ACK:
                            case EndMarks.EOT:
                            case EndMarks.ESC:
                                break;
                            default:
                                throw new IOException($"意外的字符：0x{EndMark.ToString("X2")}。预期字符：报文结束符({EndMarks.ENQ}或者{EndMarks.ACK}或者{EndMarks.EOT}或者{EndMarks.ESC})");
                        }
                        break;
                    case WorkMode.M3:
                        switch (EndMark)
                        {
                            case EndMarks.ENQ:
                            case EndMarks.NAK:
                            case EndMarks.EOT:
                            case EndMarks.ESC:
                                break;
                            default:
                                throw new IOException($"意外的字符：0x{EndMark.ToString("X2")}。预期字符：报文结束符({EndMarks.ENQ}或者{EndMarks.NAK}或者{EndMarks.EOT}或者{EndMarks.ESC})");
                        }
                        break;
                }
            }
            //验证CRC校验值
            bufferStartIndex = await ValidateCRC(stream, read_buffer, bufferStartIndex, cancellationToken, readTimeout);
            return bufferStartIndex;
        }
    }
}
