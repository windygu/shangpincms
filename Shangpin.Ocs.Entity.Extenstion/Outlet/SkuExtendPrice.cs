using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    /// <summary>
    /// sku信息扩展表
    /// </summary>
    public class SkuExtendPrice
    {
        /// <summary>
        /// 市场价
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 黄金价
        /// </summary>
        public decimal SellPrice { get; set; }

        /// <summary>
        /// 白金价
        /// </summary>
        public decimal PlatinumPrice { get; set; }

        /// <summary>
        /// 钻石价
        /// </summary>
        public decimal DiamondPrice { get; set; }

        /// <summary>
        /// 尚品价
        /// </summary>
        public decimal LimitedPrice { get; set; }

        /// <summary>
        /// 奥莱价
        /// </summary>
        public decimal LimitedVipPrice { get; set; }

        /// <summary>
        /// PC端售卖状态：0、还未进入售卖流程  1、待上架  2、已上架  3、已下架  4、已停售
        /// </summary>
        public short IsShelf { get; set; }

        /// <summary>
        /// 冻结状态：0、未冻结    1、已冻结
        /// </summary>
        public short DisabledState { get; set; }

        /// <summary>
        /// sku编码
        /// </summary>
        public string SkuNo { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string ProductNo { get; set; }

        /// <summary>
        /// 折扣价
        /// </summary>
        public decimal DiscountRate { get; set; }

        /// <summary>
        /// pc 端上架时间
        /// </summary>
        public DateTime DateShelf { get; set; }

    }
}
