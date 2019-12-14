using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateAoLaiSubjectDiscountInfo.Model
{
    [Serializable]
    public class SpfSkuPriceModel
    {
        /// <summary>
        /// 商品（SPU）编号
        /// </summary>
        public string ProductNo { get; set; }
        /// <summary>
        /// SKU编号
        /// </summary>
        public string SkuNo { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        public decimal MarketPrice { get; set; }
        /// <summary>
        /// 尚品价
        /// </summary>
        public decimal StandardPrice { get; set; }
        /// <summary>
        /// 黄金价
        /// </summary>
        public decimal GoldPrice { get; set; }
        /// <summary>
        /// 白金价
        /// </summary>
        public decimal PlatinumPrice { get; set; }
        /// <summary>
        /// 钻石价
        /// </summary>
        public decimal DiamondPrice { get; set; }
        /// <summary>
        /// 奥莱价
        /// </summary>
        public decimal OutletPrice { get; set; }
        /// <summary>
        /// 是否促销
        /// </summary>
        public short IsPromotion { get; set; }
        /// <summary>
        /// 促销价
        /// </summary>
        public decimal PromotionPrice { get; set; }
        /// <summary>
        /// 尚品折扣
        /// </summary>
        public decimal DiscountShangpin { get; set; }
        /// <summary>
        /// 奥莱折扣
        /// </summary>
        public decimal DiscountOutlet { get; set; }

        /// <summary>
        /// 商品SKU售卖状态 2上架，3下架
        /// </summary>
        public int PcSaleState { get; set; }
    }
}
