namespace Quick.SL651.Enums
{
    /// <summary>
    /// 功能码枚举
    /// </summary>
    public enum FunctionCodes : byte
    {
        /// <summary>
        /// 链路维持报
        /// </summary>
        M2F = 0x2F,
        /// <summary>
        /// 测试报
        /// </summary>
        M30 = 0x30,
        /// <summary>
        /// 均匀时段水文信息报
        /// </summary>
        M31 = 0x31,
        /// <summary>
        /// 遥测站定时报
        /// </summary>
        M32 = 0x32,
        /// <summary>
        /// 遥测站加报报
        /// </summary>
        M33 = 0x33,
        /// <summary>
        /// 遥测站小时报
        /// </summary>
        M34 = 0x34,
        /// <summary>
        /// 遥测站人工置数报
        /// </summary>
        M35 = 0x34,
        /// <summary>
        /// 中心站查询遥测站实时数据
        /// </summary>
        M37 = 0x37,
        /// <summary>
        /// 中心站查询遥测站指定要素实时数据
        /// </summary>
        M3A = 0x3A,
        /// <summary>
        /// 中心站查询水泵电机实时工作数据
        /// </summary>
        M44 = 0x44,
        /// <summary>
        /// 中心站查询遥测站软件版本
        /// </summary>
        M45 = 0x45
    }
}
