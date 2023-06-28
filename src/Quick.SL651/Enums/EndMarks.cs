namespace Quick.SL651.Enums
{
    /// <summary>
    /// 报文结束符
    /// </summary>
    public enum EndMarks : byte
    {
        /// <summary>
        /// 上行报文结束，后续无报文
        /// </summary>
        ETX = 0x03,
        /// <summary>
        /// 上行报文结束，后续有报文
        /// </summary>
        ETB = 0x17,
        /// <summary>
        /// 询问
        /// </summary>
        ENQ = 0x05,
        /// <summary>
        /// 传输结束，退出
        /// </summary>
        EOT = 0x04,
        /// <summary>
        /// 肯定确认，继续发送
        /// </summary>
        ACK = 0x06,
        /// <summary>
        /// 否定应答，反馈重发
        /// </summary>
        NAK = 0x15,
        /// <summary>
        /// 传输结束，终端保持在线
        /// </summary>
        ESC = 0x1B
    }
}
