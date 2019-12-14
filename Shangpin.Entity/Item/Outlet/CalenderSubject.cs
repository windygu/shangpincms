using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Item.Outlet
{
    /// <summary>
    /// 预售日历页码活动的JSON格式
    /// </summary>
    public class CalenderSubject
    {
        /// <summary>
        /// 活动编号
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string t { get; set; }
        /// <summary>
        /// 截断的名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string brand { get; set; }
        /// <summary>
        /// 链接
        /// </summary>
        public string href { get; set; }
        /// <summary>
        /// 活动图片
        /// </summary>
        public string img { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 活动价
        /// </summary>
        public string cprice { get; set; }
        /// <summary>
        /// 活动类型(折/元起)
        /// </summary>
        public string ff { get; set; }
        /// <summary>
        /// 活动时间
        /// </summary>
        public string time { get; set; }
        
        /// <summary>
        /// 日期开始时间
        /// </summary>
        public DateTime  DTimeBegin { get; set; }
        /// <summary>
        /// 日期结束时间
        /// </summary>
        public DateTime DTimeEnd { get; set; }
        /// <summary>
        /// 1折起，2折-折，3折，4元，5元起
        /// </summary>
        public int DiscountType { get; set; }
        public string DiscountTypeValue { get; set; }
     

    }
}
