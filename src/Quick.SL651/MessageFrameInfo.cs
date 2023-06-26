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
        /// <summary>
        /// 传输正文起始
        /// </summary>
        public readonly static byte STX = 0x02;
        /// <summary>
        /// 多包传输正文起始
        /// </summary>
        public readonly static byte SYN = 0x16;
        /// <summary>
        /// 上行报文结束，后续无报文
        /// </summary>
        public readonly static byte ETX = 0x03;
        /// <summary>
        /// 上行报文结束，后续有报文
        /// </summary>
        public readonly static byte ETB = 0x17;
        /// <summary>
        /// 询问
        /// </summary>
        public readonly static byte ENQ = 0x05;
        /// <summary>
        /// 传输结束，退出
        /// </summary>
        public readonly static byte EOT = 0x04;
        /// <summary>
        /// 肯定确认，继续发送
        /// </summary>
        public readonly static byte ACK = 0x06;
        /// <summary>
        /// 否定应答，反馈重发
        /// </summary>
        public readonly static byte NAK = 0x15;
        /// <summary>
        /// 传输结束，终端保持在线
        /// </summary>
        public readonly static byte ESC = 0x1B;

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
        public byte FunctionCode { get; set; }
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
        public byte StartMark { get; set; }
        /// <summary>
        /// 报文开始符是否是STX
        /// </summary>
        public bool IsStartMarkSTX => StartMark == STX;
        /// <summary>
        /// 报文开始符是否是SYN
        /// </summary>
        public bool IsStartMarkSYN => StartMark == STX;
        /// <summary>
        /// 报文结束符
        /// </summary>
        public byte EndMark { get; set; }
        /// <summary>
        /// 报文结束符是否是ETX
        /// </summary>
        public bool IsEndMarkETX => EndMark == ETX;
        /// <summary>
        /// 报文结束符是否是ETB
        /// </summary>
        public bool IsEndMarkETB => EndMark == ETB;
        /// <summary>
        /// 报文结束符是否是ENQ
        /// </summary>
        public bool IsEndMarkENQ => EndMark == ENQ;
        /// <summary>
        /// 报文结束符是否是EOT
        /// </summary>
        public bool IsEndMarkEOT => EndMark == EOT;
        /// <summary>
        /// 报文结束符是否是ACK
        /// </summary>
        public bool IsEndMarkACK => EndMark == ACK;
        /// <summary>
        /// 报文结束符是否是NAK
        /// </summary>
        public bool IsEndMarkNAK => EndMark == NAK;
        /// <summary>
        /// 报文结束符是否是ESC
        /// </summary>
        public bool IsEndMarkESC => EndMark == ESC;

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

        //读取报文开始符
        private async Task<int> ReadMessageStartMark(Stream stream, byte[] read_buffer, int bufferStartIndex, CancellationToken cancellationToken, int readTimeout)
        {
            bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 1, cancellationToken, readTimeout);
            StartMark = read_buffer[bufferStartIndex - 1];
            switch (WorkMode)
            {
                case WorkMode.M1:
                case WorkMode.M2:
                case WorkMode.M4:
                    if (!IsStartMarkSTX)
                        throw new IOException($"意外的字符：0x{read_buffer[bufferStartIndex - 1].ToString("X2")}。预期字符：报文开始符(传输正文起始:0x{STX.ToString("X2")})");
                    break;
                case WorkMode.M3:
                    if (!IsStartMarkSYN)
                        throw new IOException($"意外的字符：0x{read_buffer[bufferStartIndex - 1].ToString("X2")}。预期字符：报文开始符(多包传输正文起始:0x{SYN.ToString("X2")})");
                    break;
            }
            return bufferStartIndex;
        }

        //读取包总数和序列号
        private async Task<int> ReadPackageCountAndPackageIndex(Stream stream, byte[] read_buffer, int bufferStartIndex, CancellationToken cancellationToken, int readTimeout)
        {
            bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 3, cancellationToken, readTimeout);
            //解析包总数
            read_buffer[bufferStartIndex] = read_buffer[bufferStartIndex - 2];
            read_buffer[bufferStartIndex + 1] = read_buffer[bufferStartIndex - 1];
            read_buffer[bufferStartIndex + 1] &= 0xF0;
            //如果CPU是小端字节序，则交换
            if (BitConverter.IsLittleEndian)
            {
                var tmpByte = read_buffer[bufferStartIndex];
                read_buffer[bufferStartIndex] = read_buffer[bufferStartIndex + 1];
                read_buffer[bufferStartIndex + 1] = tmpByte;
            }
            PackageCount = BitConverter.ToUInt16(read_buffer, bufferStartIndex);
            PackageCount = Convert.ToUInt16(PackageCount >> 4);
            //解析包序列号
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
            PackageIndex = BitConverter.ToUInt16(read_buffer, bufferStartIndex);

            read_buffer[bufferStartIndex] = 0;
            read_buffer[bufferStartIndex + 1] = 0;
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
            if (StartMark == SYN)
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
            EndMark = read_buffer[bufferStartIndex - 1];
            if (IsUpgoing)
            {
                if (!IsEndMarkETB
                    && !IsEndMarkETX)
                    throw new IOException($"意外的字符：0x{EndMark.ToString("X2")}。预期字符：报文结束符(0x{ETX.ToString("X2")}或者0x{ETB.ToString("X2")})");
            }
            else
            {
                switch (WorkMode)
                {
                    case WorkMode.M1:
                        throw new IOException("M1工作模式没有下行帧");
                    case WorkMode.M2:
                    case WorkMode.M4:
                        if (!IsEndMarkENQ
                            && !IsEndMarkACK
                            && !IsEndMarkEOT
                            && !IsEndMarkESC)
                            throw new IOException($"意外的字符：0x{EndMark.ToString("X2")}。预期字符：报文结束符(0x{ENQ.ToString("X2")}或者0x{ACK.ToString("X2")}或者0x{EOT.ToString("X2")}或者0x{ESC.ToString("X2")})");
                        break;
                    case WorkMode.M3:
                        if (!IsEndMarkENQ
                            && !IsEndMarkNAK
                            && !IsEndMarkEOT
                            && !IsEndMarkESC)
                            throw new IOException($"意外的字符：0x{EndMark.ToString("X2")}。预期字符：报文结束符(0x{ENQ.ToString("X2")}或者0x{NAK.ToString("X2")}或者0x{EOT.ToString("X2")}或者0x{ESC.ToString("X2")})");
                        break;
                }
            }
            //验证CRC校验值
            bufferStartIndex = await ValidateCRC(stream, read_buffer, bufferStartIndex, cancellationToken, readTimeout);
            return bufferStartIndex;
        }
    }
}
