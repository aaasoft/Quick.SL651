using Quick.SL651.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Elements
{
    /// <summary>
    /// 要素定义
    /// </summary>
    public class ElementDefine
    {
        /// <summary>
        /// 要素编码
        /// </summary>
        public ElementCodes Code { get; set; }
        /// <summary>
        /// 要素名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 要素单位
        /// </summary>
        public string Unit { get; set; }
    }
}
