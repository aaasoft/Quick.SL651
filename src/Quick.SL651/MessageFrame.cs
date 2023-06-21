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

        public MessageFrame(
            byte centralStationAddress,
            string telemetryStationAddress,
            byte[] password,
            byte functionCode,
            bool isUpgoing,
            byte[] message,
            int messageStartIndex,
            int messageLength)
        {
            CentralStationAddress = centralStationAddress;
            TelemetryStationAddress = telemetryStationAddress;
            Password = password;
            FunctionCode = functionCode;
            IsUpgoing = isUpgoing;
            MessageLength = messageLength;

            //解析报文
            //message;
        }

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
    }
}
