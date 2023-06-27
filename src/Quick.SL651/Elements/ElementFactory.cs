using Quick.SL651.Enums;

namespace Quick.SL651.Elements
{
    public class ElementFactory
    {
        public static ElementFactory Instance { get; } = new ElementFactory();

        private Dictionary<ElementCodes, ElementDefine> elementDefineDict = new Dictionary<ElementCodes, ElementDefine>();

        private void register(ElementDefine elementDefine)
        {
            elementDefineDict[elementDefine.Code] = elementDefine;
        }

        /// <summary>
        /// 获取要素定义
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ElementDefine GetElementDefine(ElementCodes code)
        {
            if (elementDefineDict.TryGetValue(code, out var elementDefine))
                return elementDefine;
            return null;
        }

        /// <summary>
        /// 获取全部要素定义
        /// </summary>
        /// <returns></returns>
        public ElementDefine[] GetElementDefines()
        {
            return elementDefineDict.Values.ToArray();
        }

        private ElementFactory()
        {
            register(new ElementDefine(ElementCodes.TT, false, "观测时间引导符", null));
            register(new ElementDefine(ElementCodes.ST, false, "测站编码引导符", null));
            register(new ElementDefine(ElementCodes.RGZS, true, "人工置数", "字节"));
            register(new ElementDefine(ElementCodes.PIC, true, "图片信息", "KB"));
            register(new ElementDefine(ElementCodes.DRP, true, "1小时内每5分钟时段雨量", "0.1毫米"));
            register(new ElementDefine(ElementCodes.DRZ1, true, "1小时内5分钟间隔相对水位1", "0.01米"));
            register(new ElementDefine(ElementCodes.DRZ2, true, "1小时内5分钟间隔相对水位2", "0.01米"));
            register(new ElementDefine(ElementCodes.DRZ3, true, "1小时内5分钟间隔相对水位3", "0.01米"));
            register(new ElementDefine(ElementCodes.DRZ4, true, "1小时内5分钟间隔相对水位4", "0.01米"));
            register(new ElementDefine(ElementCodes.DRZ5, true, "1小时内5分钟间隔相对水位5", "0.01米"));
            register(new ElementDefine(ElementCodes.DRZ6, true, "1小时内5分钟间隔相对水位6", "0.01米"));
            register(new ElementDefine(ElementCodes.DRZ7, true, "1小时内5分钟间隔相对水位7", "0.01米"));
            register(new ElementDefine(ElementCodes.DRZ8, true, "1小时内5分钟间隔相对水位8", "0.01米"));
            register(new ElementDefine(ElementCodes.DATA, false, "流速批量数据传输", null));
            register(new ElementDefine(ElementCodes.AC, true, "断面面积", "平方米"));
            register(new ElementDefine(ElementCodes.Al, true, "瞬时气温", "摄氏度"));
            register(new ElementDefine(ElementCodes.C, true, "瞬时水温", "摄氏度"));
            register(new ElementDefine(ElementCodes.DRxnn, true, "时间步长码9", ""));
            register(new ElementDefine(ElementCodes.DT, true, "时段长，降水、引排水、抽水历时", "小时.分钟"));
            register(new ElementDefine(ElementCodes.ED, true, "日蒸发量", "毫米"));
            register(new ElementDefine(ElementCodes.EJ, true, "当前蒸发", "毫米"));
            register(new ElementDefine(ElementCodes.FL, true, "气压", "百帕"));
            register(new ElementDefine(ElementCodes.GH, true, "闸坝、水库闸门开启高度", "米"));
            register(new ElementDefine(ElementCodes.GN, false, "输水设备、闸门(组)编号", null));
            register(new ElementDefine(ElementCodes.GS, false, "输水设备类别", null));
            register(new ElementDefine(ElementCodes.GT, true, "水库、闸坝闸门开启孔数", "孔"));
            register(new ElementDefine(ElementCodes.GTP, true, "地温", "摄氏度"));
            register(new ElementDefine(ElementCodes.H, true, "地下水瞬时埋深", "米"));
            register(new ElementDefine(ElementCodes.HW, true, "波浪高度", "米"));
            register(new ElementDefine(ElementCodes.M10, true, "10厘米处土壤含水量", "百分比"));
            register(new ElementDefine(ElementCodes.M20, true, "20厘米处土壤含水量", "百分比"));
            register(new ElementDefine(ElementCodes.M30, true, "30厘米处土壤含水量", "百分比"));
            register(new ElementDefine(ElementCodes.M40, true, "40厘米处土壤含水量", "百分比"));
            register(new ElementDefine(ElementCodes.M50, true, "50厘米处土壤含水量", "百分比"));
            register(new ElementDefine(ElementCodes.M60, true, "60厘米处土壤含水量", "百分比"));
            register(new ElementDefine(ElementCodes.M80, true, "80厘米处土壤含水量", "百分比"));
            register(new ElementDefine(ElementCodes.M100, true, "100厘米处土壤含水量", "百分比"));
            register(new ElementDefine(ElementCodes.MST, true, "湿度", "百分比"));
            register(new ElementDefine(ElementCodes.NS, true, "开机台数", "台"));
            register(new ElementDefine(ElementCodes.P1, true, "1小时时段降水量", "毫米"));
            register(new ElementDefine(ElementCodes.P2, true, "2小时时段降水量", "毫米"));
            register(new ElementDefine(ElementCodes.P3, true, "3小时时段降水量", "毫米"));
            register(new ElementDefine(ElementCodes.P6, true, "6小时时段降水量", "毫米"));
            register(new ElementDefine(ElementCodes.P12, true, "12小时时段降水量", "毫米"));
            register(new ElementDefine(ElementCodes.PD, true, "日降水量", "毫米"));
            register(new ElementDefine(ElementCodes.PJ, true, "当前降水量", "毫米"));
            register(new ElementDefine(ElementCodes.PN01, true, "1分钟时段降水量", "毫米"));
            register(new ElementDefine(ElementCodes.PN05, true, "5分钟时段降水量", "毫米"));
            register(new ElementDefine(ElementCodes.PN10, true, "10分钟时段降水量", "毫米"));
            register(new ElementDefine(ElementCodes.PN30, true, "30分钟时段降水量", "毫米"));
            register(new ElementDefine(ElementCodes.PR, true, "暴雨量", "毫米"));
            register(new ElementDefine(ElementCodes.PT, true, "降水量累计值", "毫米"));
            register(new ElementDefine(ElementCodes.Q, true, "瞬时流量、抽水流量", "立方米/秒"));
            register(new ElementDefine(ElementCodes.Q1, true, "取(排)水口流量1", "立方米/秒"));
            register(new ElementDefine(ElementCodes.Q2, true, "取(排)水口流量2", "立方米/秒"));
            register(new ElementDefine(ElementCodes.Q3, true, "取(排)水口流量3", "立方米/秒"));
            register(new ElementDefine(ElementCodes.Q4, true, "取(排)水口流量4", "立方米/秒"));
            register(new ElementDefine(ElementCodes.Q5, true, "取(排)水口流量5", "立方米/秒"));
            register(new ElementDefine(ElementCodes.Q6, true, "取(排)水口流量6", "立方米/秒"));
            register(new ElementDefine(ElementCodes.Q7, true, "取(排)水口流量7", "立方米/秒"));
            register(new ElementDefine(ElementCodes.Q8, true, "取(排)水口流量8", "立方米/秒"));
            register(new ElementDefine(ElementCodes.QA, true, "总出库流量、过闸总流量", "立方米/秒"));
            register(new ElementDefine(ElementCodes.QZ, true, "输水设备流量、过闸(组)流量", "立方米/秒"));
            register(new ElementDefine(ElementCodes.SW, true, "输沙量", "万吨"));
            register(new ElementDefine(ElementCodes.UC, false, "风向", null));
            register(new ElementDefine(ElementCodes.UE, true, "风力(级)", null));
            register(new ElementDefine(ElementCodes.US, true, "风速", "米/秒"));
            register(new ElementDefine(ElementCodes.VA, true, "断面平均流速", "米/秒"));
            register(new ElementDefine(ElementCodes.VJ, true, "当前瞬时流速", "米/秒"));
            register(new ElementDefine(ElementCodes.VT, true, "电源电压", "伏特"));
            register(new ElementDefine(ElementCodes.Z, true, "瞬时河道水位、潮位", "米"));
            register(new ElementDefine(ElementCodes.ZB, true, "库(闸、站)下水位", "米"));
            register(new ElementDefine(ElementCodes.ZU, true, "库(闸、站)上水位", "米"));
            register(new ElementDefine(ElementCodes.Z1, true, "取(排)水口水位1", "米"));
            register(new ElementDefine(ElementCodes.Z2, true, "取(排)水口水位2", "米"));
            register(new ElementDefine(ElementCodes.Z3, true, "取(排)水口水位3", "米"));
            register(new ElementDefine(ElementCodes.Z4, true, "取(排)水口水位4", "米"));
            register(new ElementDefine(ElementCodes.Z5, true, "取(排)水口水位5", "米"));
            register(new ElementDefine(ElementCodes.Z6, true, "取(排)水口水位6", "米"));
            register(new ElementDefine(ElementCodes.Z7, true, "取(排)水口水位7", "米"));
            register(new ElementDefine(ElementCodes.Z8, true, "取(排)水口水位8", "米"));
            register(new ElementDefine(ElementCodes.SQ, true, "含沙量", "千克/立方米"));
            register(new ElementDefine(ElementCodes.ZT, true, "遥测站状态及报警信息(定义见表58)", ""));
            register(new ElementDefine(ElementCodes.pH, true, "pH值", null));
            register(new ElementDefine(ElementCodes.DO, true, "溶解氧", "毫克/升"));
            register(new ElementDefine(ElementCodes.COND, true, "电导率", "微西门/厘米"));
            register(new ElementDefine(ElementCodes.TURB, true, "浊度", "度"));
            register(new ElementDefine(ElementCodes.CODMN, true, "高锰酸盐指数", "毫克/升"));
            register(new ElementDefine(ElementCodes.REDOX, true, "氧化还原电位", "毫伏"));
            register(new ElementDefine(ElementCodes.NH4N, true, "氨氮", "毫克/升"));
            register(new ElementDefine(ElementCodes.TP, true, "总磷", "毫克/升"));
            register(new ElementDefine(ElementCodes.TN, true, "总氮", "毫克/升"));
            register(new ElementDefine(ElementCodes.TOC, true, "总有机碳", "毫克/升"));
            register(new ElementDefine(ElementCodes.CU, true, "铜", "毫克/升"));
            register(new ElementDefine(ElementCodes.ZN, true, "锌", "毫克/升"));
            register(new ElementDefine(ElementCodes.SE, true, "硒", "毫克/升"));
            register(new ElementDefine(ElementCodes.AS, true, "砷", "毫克/升"));
            register(new ElementDefine(ElementCodes.THG, true, "总汞", "毫克/升"));
            register(new ElementDefine(ElementCodes.CD, true, "镉", "毫克/升"));
            register(new ElementDefine(ElementCodes.PB, true, "铅", "毫克/升"));
            register(new ElementDefine(ElementCodes.CHLA, true, "叶绿素a", "毫克/升"));
            register(new ElementDefine(ElementCodes.WP1, true, "水压1", "千帕"));
            register(new ElementDefine(ElementCodes.WP2, true, "水压2", "千帕"));
            register(new ElementDefine(ElementCodes.WP3, true, "水压3", "千帕"));
            register(new ElementDefine(ElementCodes.WP4, true, "水压4", "千帕"));
            register(new ElementDefine(ElementCodes.WP5, true, "水压5", "千帕"));
            register(new ElementDefine(ElementCodes.WP6, true, "水压6", "千帕"));
            register(new ElementDefine(ElementCodes.WP7, true, "水压7", "千帕"));
            register(new ElementDefine(ElementCodes.WP8, true, "水压8", "千帕"));
            register(new ElementDefine(ElementCodes.SYL1, true, "水表1剩余水量", "立方米"));
            register(new ElementDefine(ElementCodes.SYL2, true, "水表2剩余水量", "立方米"));
            register(new ElementDefine(ElementCodes.SYL3, true, "水表3剩余水量", "立方米"));
            register(new ElementDefine(ElementCodes.SYL4, true, "水表4剩余水量", "立方米"));
            register(new ElementDefine(ElementCodes.SYL5, true, "水表5剩余水量", "立方米"));
            register(new ElementDefine(ElementCodes.SYL6, true, "水表6剩余水量", "立方米"));
            register(new ElementDefine(ElementCodes.SYL7, true, "水表7剩余水量", "立方米"));
            register(new ElementDefine(ElementCodes.SYL8, true, "水表8剩余水量", "立方米"));
            register(new ElementDefine(ElementCodes.SBL1, true, "水表1每小时水量", "立方米/小时"));
            register(new ElementDefine(ElementCodes.SBL2, true, "水表2每小时水量", "立方米/小时"));
            register(new ElementDefine(ElementCodes.SBL3, true, "水表3每小时水量", "立方米/小时"));
            register(new ElementDefine(ElementCodes.SBL4, true, "水表4每小时水量", "立方米/小时"));
            register(new ElementDefine(ElementCodes.SBL5, true, "水表5每小时水量", "立方米/小时"));
            register(new ElementDefine(ElementCodes.SBL6, true, "水表6每小时水量", "立方米/小时"));
            register(new ElementDefine(ElementCodes.SBL7, true, "水表7每小时水量", "立方米/小时"));
            register(new ElementDefine(ElementCodes.SBL8, true, "水表8每小时水量", "立方米/小时"));
            register(new ElementDefine(ElementCodes.VTA, true, "交流A相电压", "伏特"));
            register(new ElementDefine(ElementCodes.VTB, true, "交流B相电压", "伏特"));
            register(new ElementDefine(ElementCodes.VTC, true, "交流C相电压", "伏特"));
            register(new ElementDefine(ElementCodes.VIA, true, "交流A相电流", "安培"));
            register(new ElementDefine(ElementCodes.VIB, true, "交流B相电流", "安培"));
            register(new ElementDefine(ElementCodes.VIC, true, "交流C相电流", "安培"));
        }
    }
}
