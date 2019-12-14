using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Orders
{
    public class RefundInfo
    {
        public string OrderNo { get; set; }
        public short Status { get; set; }
        public short PayTypeId { get; set; }
        public DateTime DateComplete { get; set; }
        public string ProductNo { get; set; }
        public string SkuNo { get; set; }
        public string ReturnChangeFlag { get; set; }
        public string OrderDetailNo { get; set; }
        /// <summary>
        /// 是否是换货订单
        /// </summary>
        public int IsTradeOrder { get; set; }
    }

    
}
