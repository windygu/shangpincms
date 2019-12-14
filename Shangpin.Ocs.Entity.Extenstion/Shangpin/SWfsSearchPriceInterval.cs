using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    /// <summary>
    /// 搜索价格设置
    /// </summary>
    public class SWfsSearchPriceInterval
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int IntervalId{get;set;}

        /// <summary>
        /// 分类编号
        /// </summary>
        public string CategoryNo{get;set;}

        /// <summary>
        /// 价格最小值 
        /// </summary>
        public string MinPrice { get; set; }

        /// <summary>
        /// 价格最大值 
        /// </summary>
        public string MaxPrice { get; set; }

        /// <summary>
        /// 显示状态(0显示，1不显示，默认为0) 
        /// </summary>
        public int Status{get;set;}
 
        /// <summary>
        /// 创建时间 
        /// </summary>
        public DateTime CreateDate{get;set;}

        /// <summary>
        /// 操作人 
        /// </summary>
        public string OperatorUserId { get; set; }
 

    }
}
