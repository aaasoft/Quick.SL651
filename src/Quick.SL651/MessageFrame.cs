using System;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651
{
    public class MessageFrame
    {
        /// <summary>
        /// ASCII编码帧起始符
        /// </summary>
        public readonly static byte[] ASCII_S0H = { 0x01 };
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
        /// 中心站地址
        /// </summary>
        public byte CentralStationAddress { get; private set; }
        /// <summary>
        /// 遥测站地址
        /// </summary>
        public string TelemetryStationAddress { get; private set; }
        /// <summary>
        /// 密码
        /// </summary>
        public byte[] Password { get; private set; }
        /// <summary>
        /// 功能码
        /// </summary>
        public byte FunctionCode { get; private set; }
        /// <summary>
        /// 是否上行
        /// </summary>
        public bool IsUpgoing { get; private set; }
        /// <summary>
        /// 报文长度
        /// </summary>
        public int MessageLength { get; private set; }

        public static MessageFrame Parse(FrameEncoding frameEncoding, Span<byte> data)
        {
            //帧起始符
            var frameHeader = frameEncoding == FrameEncoding.ASCII ? ASCII_S0H : HEX_BCD_S0H;
            //将ASCII编码的帧转换为HEX/BCD编码
            if (frameEncoding == FrameEncoding.ASCII)
            {
                for (var i = 0; i < data.Length; i += 2)
                {
                    var str = Encoding.ASCII.GetString(data.Slice(i, 2));
                    var b = byte.Parse(str, System.Globalization.NumberStyles.HexNumber);
                    data[i / 2] = b;
                }
                data = data.Slice(0, data.Length / 2);
            }
            //校验CRC
            var crcResult = Crc16.Hash(data.Slice(0, data.Length - 2));
            if (!data.EndsWith(crcResult))
                throw new IOException("报文帧CRC检验失败！");
            //去除帧起始符
            data = data.Slice(frameHeader.Length);
            //中心站地址
            var centralStationAddress = data[0];
            data = data.Slice(1);
            //遥测站地址
            var telemetryStationAddressStringBuilder = new StringBuilder();
            for (var i = 0; i < 5; i++)
            {
                telemetryStationAddressStringBuilder.Append(data[i].ToString("X2"));
            }
            var telemetryStationAddress = telemetryStationAddressStringBuilder.ToString();
            data = data.Slice(5);
            //密码
            var password = data.Slice(0, 2).ToArray();
            data = data.Slice(2);
            //功能码
            var functionCode = data[0];
            data = data.Slice(1);
            //报文上下行标识和报文长度
            var upgoingByte = data[0];
            upgoingByte &= 0xF0;
            var isUpgoing = upgoingByte == 0;
            data[0] &= 0x0F;
            var messageLengthSpan = data.Slice(0, 2);
            //如果CPU是小端字节序，则交换
            if (BitConverter.IsLittleEndian)
                messageLengthSpan.Reverse();
            var messageLength = BitConverter.ToInt16(messageLengthSpan);
            data = data.Slice(2);
            //报文开始
            if (data[0] != STX)
                throw new IOException($"意外的字符：0x{data[0].ToString("X2")}。预期字符：报文开始符(0x{STX.ToString("X2")})");
            data = data.Slice(1);
            //报文正文
            var messageSpan = data.Slice(0, messageLength);

            //解析报文正文

            data = data.Slice(messageLength);
            //报文结束
            if (data[0] != ETX && data[0] != ETB)
                throw new IOException($"意外的字符：0x{data[0].ToString("X2")}。预期字符：报文结束符(0x{ETX.ToString("X2")}或者0x{ETB.ToString("X2")})");
            data = data.Slice(1);
            //解析完成
            return new MessageFrame()
            {
                CentralStationAddress = centralStationAddress,
                TelemetryStationAddress = telemetryStationAddress,
                Password = password,
                FunctionCode = functionCode,
                IsUpgoing = isUpgoing,
                MessageLength = messageLength
            };
        }
    }
}
