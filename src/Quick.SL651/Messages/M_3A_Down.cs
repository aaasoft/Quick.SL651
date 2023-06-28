using Quick.SL651.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    /// <summary>
    /// 中心站查询遥测站指定要素实时数据下行报文
    /// </summary>
    public class M_3A_Down : Message
    {
        public ElementCodes[] ElementCodes { get; set; }
        public M_3A_Down()
        {
        }

        public override int Write(Stream stream)
        {
            var ret = base.Write(stream);
            foreach (var code in ElementCodes)
            {
                stream.WriteByte((byte)code);
                ret++;
            }
            return ret;
        }
    }
}
