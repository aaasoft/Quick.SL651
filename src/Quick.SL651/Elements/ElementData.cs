﻿using Quick.SL651.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Elements
{
    /// <summary>
    /// 要素值
    /// </summary>
    public class ElementData
    {
        /// <summary>
        /// 要素编码
        /// </summary>
        public ElementCodes Code { get; set; }
        /// <summary>
        /// 数据字节数
        /// </summary>
        public int DataBytesCount { get; set; }
        /// <summary>
        /// 小数位数
        /// </summary>
        public int Scale { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public byte[] Value { get; set; }
        /// <summary>
        /// 字符串值
        /// </summary>
        public string StringValue { get; set; }
        /// <summary>
        /// 数字值
        /// </summary>
        public double NumberValue { get; set; }

        public Span<byte> Load(Span<byte> span)
        {
            //读取要素编码
            Code = (ElementCodes)span[0];
            span = span.Slice(1);
            //读取数据字节数和小数位数
            var b = span[0];
            DataBytesCount = b >> 3;
            b = span[0];
            b &= 0b_0000_0111;
            Scale = b;
            span = span.Slice(1);
            //读取数据
            var valueSpan = span.Slice(0, DataBytesCount);
            Value = valueSpan.ToArray();
            StringValue = valueSpan.BCD_Decode();
            double numberValue;
            if (double.TryParse(StringValue, out numberValue))
            {
                if (Scale > 0)
                    numberValue = numberValue / Math.Pow(10, Scale);
                NumberValue = numberValue;
            }
            span = span.Slice(DataBytesCount);
            return span;
        }
    }
}