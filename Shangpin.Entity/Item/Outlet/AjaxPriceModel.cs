using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Item.Outlet
{
    /// <summary>
    /// 活动列表使用到的实体类
    /// </summary>
    public class AjaxPriceModel
    {
        /// <summary>
        /// 限时价格
        /// </summary>
        public string LimitedVipPrice { get; set; }
        /// <summary>
        /// 市场价格
        /// </summary>
        public string MarketPrice { get; set; }
       /// <summary>
       /// 是否有库存
       /// </summary>
        public bool IsInvented { get; set; }

        public string ColorSizeHtml { get; set; }

    }
}
