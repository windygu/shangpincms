using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Item.Outlet
{
    /// <summary>
    /// 预售日历页码活动的JSON格式
    /// </summary>
    public class CalenderSubjectInfo
    { 
        /// <summary>
        /// 链接
        /// </summary>
        public string href { get; set; }  
        /// <summary>
        /// 日期开始时间
        /// </summary>
        public DateTime DateBegin { get; set; }
        /// <summary>
        /// 日期结束时间
        /// </summary>
        public DateTime DateEnd { get; set; }
        /// <summary>
        /// 1折起，2折-折，3折，4元，5元起
        /// </summary>
        public int DiscountType { get; set; }
     

        /*以下是新加字段*/
        /// <summary>
        /// 活动编号
        /// </summary>
        public string SubjectNo { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string SubjectName { get; set; }
        /// <summary>
        /// 打折值
        /// </summary>
        public string DiscountTypeValue { get; set; }

        /// <summary>
        /// 活动日期
        /// </summary>
        public DateTime SubjectDateCreate { get; set; }
        /// <summary>
        /// 图片编号
        /// </summary>
        public string BelongsSubjectPic { get; set; }

    }
}
