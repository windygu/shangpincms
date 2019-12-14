using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Customers
{
    public class TicketProduct
    {
        /// <summary>
        /// 购买状态 0未开始 1抢购中 2已结束
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductNo { get; set; }

        /// <summary>
        /// 卖出数量
        /// </summary>
        public int BuyedCount { get; set; }
        /// <summary>
        /// 显示样式
        /// </summary>
        public string CssClass { get; set; }
    }
}
