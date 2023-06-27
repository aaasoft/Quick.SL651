using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Enums
{
    /// <summary>
    /// 要素编码枚举
    /// </summary>
    public enum ElementCodes : byte
    {
        /// <summary>
        /// 观测时间引导符
        /// </summary>
        TT = 0xF0,
        /// <summary>
        /// 测站编码引导符b
        /// </summary>
        ST = 0xF1,
        /// <summary>
        /// 人工置数°
        /// </summary>
        RGZS = 0xF2,
        /// <summary>
        /// 图片信息
        /// </summary>
        PIC = 0xF3,
        /// <summary>
        /// 1小时内每5分钟时段雨量  (每组雨量占1字节HEX, 最大值25.4毫米，数据中不含小数点；FFH表示非法数据。)
        /// </summary>
        DRP = 0xF4,
        /// <summary>
        /// 1小时内5分钟间隔相对水位1(每组水位占2字节HEX, 分辨力是为厘米，最大值为655.34米，数据中不含小数点；FFH表示非法数据); 对于河道、闸坝(泵)站分别表示河道水位、闸(站)上水位。
        /// </summary>
        DRZ1 = 0xF5,
        /// <summary>
        /// 1小时内5分钟间隔相对水位2;对于闸坝(泵)站表示闸(站)下水位。
        /// </summary>
        DRZ2 = 0xF6,
        /// <summary>
        /// 1小时内5分钟间隔相对水位3
        /// </summary>
        DRZ3 = 0xF7,
        /// <summary>
        /// 1小时内5分钟间隔相对水位4
        /// </summary>
        DRZ4 = 0xF8,
        /// <summary>
        /// 1小时内5分钟间隔相对水位5
        /// </summary>
        DRZ5 = 0xF9,
        /// <summary>
        /// 1小时内5分钟间隔相对水位6
        /// </summary>
        DRZ6 = 0xFA,
        /// <summary>
        /// 1小时内5分钟间隔相对水位7
        /// </summary>
        DRZ7 = 0xFB,
        /// <summary>
        /// 1小时内5分钟间隔相对水位8
        /// </summary>
        DRZ8 = 0xFC,
        /// <summary>
        /// 流速批量数据传输
        /// </summary>
        DATA = 0xFD,
        /// <summary>
        /// 断面面积
        /// </summary>
        AC = 0x01,
        /// <summary>
        /// 瞬时气温
        /// </summary>
        Al = 0x02,
        /// <summary>
        /// 瞬时水温
        /// </summary>
        C = 0x03,
        /// <summary>
        /// 时间步长码9
        /// </summary>
        DRxnn = 0x04,
        /// <summary>
        /// 时段长，降水、引排水、抽水历时
        /// </summary>
        DT = 0x05,
        /// <summary>
        /// 日蒸发量
        /// </summary>
        ED = 0x06,
        /// <summary>
        /// 当前蒸发
        /// </summary>
        EJ = 0x07,
        /// <summary>
        /// 气压
        /// </summary>
        FL = 0x08,
        /// <summary>
        /// 闸坝、水库闸门开启高度
        /// </summary>
        GH = 0x09,
        /// <summary>
        /// 输水设备、闸门(组)编号
        /// </summary>
        GN = 0x0A,
        /// <summary>
        /// 输水设备类别
        /// </summary>
        GS = 0x0B,
        /// <summary>
        /// 水库、闸坝闸门开启孔数
        /// </summary>
        GT = 0x0C,
        /// <summary>
        /// 地温
        /// </summary>
        GTP = 0x0D,
        /// <summary>
        /// 地下水瞬时埋深
        /// </summary>
        H = 0x0E,
        /// <summary>
        /// 波浪高度
        /// </summary>
        HW = 0x0F,
        /// <summary>
        /// 10厘米处土壤含水量
        /// </summary>
        M10 = 0x10,
        /// <summary>
        /// 20厘米处土壤含水量
        /// </summary>
        M20 = 0x11,
        /// <summary>
        /// 30厘米处土壤含水量
        /// </summary>
        M30 = 0x12,
        /// <summary>
        /// 40厘米处土壤含水量
        /// </summary>
        M40 = 0x13,
        /// <summary>
        /// 50厘米处土壤含水量
        /// </summary>
        M50 = 0x14,
        /// <summary>
        /// 60厘米处土壤含水量
        /// </summary>
        M60 = 0x15,
        /// <summary>
        /// 80厘米处土壤含水量
        /// </summary>
        M80 = 0x16,
        /// <summary>
        /// 100厘米处土壤含水量
        /// </summary>
        M100 = 0x17,
        /// <summary>
        /// 湿度
        /// </summary>
        MST = 0x18,
        /// <summary>
        /// 开机台数
        /// </summary>
        NS = 0x19,
        /// <summary>
        /// 1小时时段降水量
        /// </summary>
        P1 = 0x1A,
        /// <summary>
        /// 2小时时段降水量
        /// </summary>
        P2 = 0x1B,
        /// <summary>
        /// 3小时时段降水量
        /// </summary>
        P3 = 0x1C,
        /// <summary>
        /// 6小时时段降水量
        /// </summary>
        P6 = 0x1D,
        /// <summary>
        /// 12小时时段降水量
        /// </summary>
        P12 = 0x1E,
        /// <summary>
        /// 日降水量
        /// </summary>
        PD = 0x1F,
        /// <summary>
        /// 当前降水量
        /// </summary>
        PJ = 0x20,
        /// <summary>
        /// 1分钟时段降水量
        /// </summary>
        PN01 = 0x21,
        /// <summary>
        /// 5分钟时段降水量
        /// </summary>
        PN05 = 0x22,
        /// <summary>
        /// 10分钟时段降水量
        /// </summary>
        PN10 = 0x23,
        /// <summary>
        /// 30分钟时段降水量
        /// </summary>
        PN30 = 0x24,
        /// <summary>
        /// 暴雨量
        /// </summary>
        PR = 0x25,
        /// <summary>
        /// 降水量累计值
        /// </summary>
        PT = 0x26,
        /// <summary>
        /// 瞬时流量、抽水流量
        /// </summary>
        Q = 0x27,
        /// <summary>
        /// 取(排)水口流量1
        /// </summary>
        Q1 = 0x28,
        /// <summary>
        /// 取(排)水口流量2
        /// </summary>
        Q2 = 0x29,
        /// <summary>
        /// 取(排)水口流量3
        /// </summary>
        Q3 = 0x2A,
        /// <summary>
        /// 取(排)水口流量4
        /// </summary>
        Q4 = 0x2B,
        /// <summary>
        /// 取(排)水口流量5
        /// </summary>
        Q5 = 0x2C,
        /// <summary>
        /// 取(排)水口流量6
        /// </summary>
        Q6 = 0x2D,
        /// <summary>
        /// 取(排)水口流量7
        /// </summary>
        Q7 = 0x2E,
        /// <summary>
        /// 取(排)水口流量8
        /// </summary>
        Q8 = 0x2F,
        /// <summary>
        /// 总出库流量、过闸总流量
        /// </summary>
        QA = 0x30,
        /// <summary>
        /// 输水设备流量、过闸(组)流量
        /// </summary>
        QZ = 0x31,
        /// <summary>
        /// 输沙量
        /// </summary>
        SW = 0x32,
        /// <summary>
        /// 风向
        /// </summary>
        UC = 0x33,
        /// <summary>
        /// 风力(级)
        /// </summary>
        UE = 0x34,
        /// <summary>
        /// 风速
        /// </summary>
        US = 0x35,
        /// <summary>
        /// 断面平均流速
        /// </summary>
        VA = 0x36,
        /// <summary>
        /// 当前瞬时流速
        /// </summary>
        VJ = 0x37,
        /// <summary>
        /// 电源电压
        /// </summary>
        VT = 0x38,
        /// <summary>
        /// 瞬时河道水位、潮位
        /// </summary>
        Z = 0x39,
        /// <summary>
        /// 库(闸、站)下水位
        /// </summary>
        ZB = 0x3A,
        /// <summary>
        /// 库(闸、站)上水位
        /// </summary>
        ZU = 0x3B,
        /// <summary>
        /// 取(排)水口水位1
        /// </summary>
        Z1 = 0x3C,
        /// <summary>
        /// 取(排)水口水位2
        /// </summary>
        Z2 = 0x3D,
        /// <summary>
        /// 取(排)水口水位3
        /// </summary>
        Z3 = 0x3E,
        /// <summary>
        /// 取(排)水口水位4
        /// </summary>
        Z4 = 0x3F,
        /// <summary>
        /// 取(排)水口水位5
        /// </summary>
        Z5 = 0x40,
        /// <summary>
        /// 取(排)水口水位6
        /// </summary>
        Z6 = 0x41,
        /// <summary>
        /// 取(排)水口水位7
        /// </summary>
        Z7 = 0x42,
        /// <summary>
        /// 取(排)水口水位8
        /// </summary>
        Z8 = 0x43,
        /// <summary>
        /// 含沙量
        /// </summary>
        SQ = 0x44,
        /// <summary>
        /// 遥测站状态及报警信息(定义见表58)
        /// </summary>
        ZT = 0x45,
        /// <summary>
        /// pH值
        /// </summary>
        pH = 0x46,
        /// <summary>
        /// 溶解氧
        /// </summary>
        DO = 0x47,
        /// <summary>
        /// 电导率
        /// </summary>
        COND = 0x48,
        /// <summary>
        /// 浊度
        /// </summary>
        TURB = 0x49,
        /// <summary>
        /// 高锰酸盐指数
        /// </summary>
        CODMN = 0x4A,
        /// <summary>
        /// 氧化还原电位
        /// </summary>
        REDOX = 0x4B,
        /// <summary>
        /// 氨氮
        /// </summary>
        NH4N = 0x4C,
        /// <summary>
        /// 总磷
        /// </summary>
        TP = 0x4D,
        /// <summary>
        /// 总氮
        /// </summary>
        TN = 0x4E,
        /// <summary>
        /// 总有机碳
        /// </summary>
        TOC = 0x4F,
        /// <summary>
        /// 铜
        /// </summary>
        CU = 0x50,
        /// <summary>
        /// 锌
        /// </summary>
        ZN = 0x51,
        /// <summary>
        /// 硒
        /// </summary>
        SE = 0x52,
        /// <summary>
        /// 砷
        /// </summary>
        AS = 0x53,
        /// <summary>
        /// 总汞
        /// </summary>
        THG = 0x54,
        /// <summary>
        /// 镉
        /// </summary>
        CD = 0x55,
        /// <summary>
        /// 铅
        /// </summary>
        PB = 0x56,
        /// <summary>
        /// 叶绿素a
        /// </summary>
        CHLA = 0x57,
        /// <summary>
        /// 水压1
        /// </summary>
        WP1 = 0x58,
        /// <summary>
        /// 水压2
        /// </summary>
        WP2 = 0x59,
        /// <summary>
        /// 水压3
        /// </summary>
        WP3 = 0x5A,
        /// <summary>
        /// 水压4
        /// </summary>
        WP4 = 0x5B,
        /// <summary>
        /// 水压5
        /// </summary>
        WP5 = 0x5C,
        /// <summary>
        /// 水压6
        /// </summary>
        WP6 = 0x5D,
        /// <summary>
        /// 水压7
        /// </summary>
        WP7 = 0x5E,
        /// <summary>
        /// 水压8
        /// </summary>
        WP8 = 0x5F,
        /// <summary>
        /// 水表1剩余水量
        /// </summary>
        SYL1 = 0x60,
        /// <summary>
        /// 水表2剩余水量
        /// </summary>
        SYL2 = 0x61,
        /// <summary>
        /// 水表3剩余水量
        /// </summary>
        SYL3 = 0x62,
        /// <summary>
        /// 水表4剩余水量
        /// </summary>
        SYL4 = 0x63,
        /// <summary>
        /// 水表5剩余水量
        /// </summary>
        SYL5 = 0x64,
        /// <summary>
        /// 水表6剩余水量
        /// </summary>
        SYL6 = 0x65,
        /// <summary>
        /// 水表7剩余水量
        /// </summary>
        SYL7 = 0x66,
        /// <summary>
        /// 水表8剩余水量
        /// </summary>
        SYL8 = 0x67,
        /// <summary>
        /// 水表1每小时水量
        /// </summary>
        SBL1 = 0x68,
        /// <summary>
        /// 水表2每小时水量
        /// </summary>
        SBL2 = 0x69,
        /// <summary>
        /// 水表3每小时水量
        /// </summary>
        SBL3 = 0x6A,
        /// <summary>
        /// 水表4每小时水量
        /// </summary>
        SBL4 = 0x6B,
        /// <summary>
        /// 水表5每小时水量
        /// </summary>
        SBL5 = 0x6C,
        /// <summary>
        /// 水表6每小时水量
        /// </summary>
        SBL6 = 0x6D,
        /// <summary>
        /// 水表7每小时水量
        /// </summary>
        SBL7 = 0x6E,
        /// <summary>
        /// 水表8每小时水量
        /// </summary>
        SBL8 = 0x6F,
        /// <summary>
        /// 交流A相电压
        /// </summary>
        VTA = 0x70,
        /// <summary>
        /// 交流B相电压
        /// </summary>
        VTB = 0x71,
        /// <summary>
        /// 交流C相电压
        /// </summary>
        VTC = 0x72,
        /// <summary>
        /// 交流A相电流
        /// </summary>
        VIA = 0x73,
        /// <summary>
        /// 交流B相电流
        /// </summary>
        VIB = 0x74,
        /// <summary>
        /// 交流C相电流
        /// </summary>
        VIC = 0x75
    }
}
