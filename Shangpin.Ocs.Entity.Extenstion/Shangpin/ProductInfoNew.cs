using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Common;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class ProductInfoNew : PagingEntityBase
    {
        public string ProductNo { get; set; }
        public string GoodsNo { get; set; }
        public string ProductName { get; set; }
        public string ProductPicFile { get; set; }

        //市场价
        public decimal MarketPrice { get; set; }

        //奥莱价
        public decimal LimitedVipPrice { get; set; }

        /// 白金会员价
        public decimal PlatinumPrice { get; set; }

        /// 钻石会员价
        public decimal DiamondPrice { get; set; }

        /// 普通会员价
        public decimal LimitedPrice { get; set; }

        //促销价
        public decimal PromotionPrice { get; set; }
        // 尚品价--黄金价
        public decimal SellPrice { get; set; }

        //上架时间
        public DateTime DateShelf { get; set; }

        public string BrandNo { get; set; }
        public string BrandEnName { get; set; }
        public string BrandCnName { get; set; }
        public int Quantity { get; set; }
        public int LockQuantity { get; set; }
        public int Status { get; set; }

       
        //库龄
        public int InventoryAge { get; set; }

        public short IsShelf { get; set; }

        //------------专题商品扩展------------//
        public decimal sellBid { get; set; }// (Convert.ToDecimal(sellPrice) - Convert.ToDecimal(bid)).ToString();
        public decimal marketBid { get; set; }// (Convert.ToDecimal(marketPrice) - Convert.ToDecimal(bid)).ToString();
        public decimal platinumBid { get; set; } //(Convert.ToDecimal(platinumPrice) - Convert.ToDecimal(bid)).ToString();
        public decimal diamondBid { get; set; } //(Convert.ToDecimal(diamondPrice) - Convert.ToDecimal(bid)).ToString();
        public decimal limitedBid { get; set; } //(Convert.ToDecimal(limitedPrice) - Convert.ToDecimal(bid)).ToString();
        public decimal limitedVipBid { get; set; } //(Convert.ToDecimal(limitedVipPrice) - Convert.ToDecimal(bid)).ToString();

        public decimal selBidRate { get; set; }//Convert.ToDecimal(Math.Round((Convert.ToDecimal(sellBid) / Convert.ToDecimal(sellPrice)) * 100, 2)).ToString() + "%";
        public decimal marketBidRate { get; set; }// Convert.ToDecimal(Math.Round((Convert.ToDecimal(marketBid) / Convert.ToDecimal(marketPrice)) * 100, 2)).ToString() + "%";
        public decimal platinumBidRate { get; set; }// Convert.ToDecimal(Math.Round((Convert.ToDecimal(platinumBid) / Convert.ToDecimal(platinumPrice)) * 100, 2)).ToString() + "%";
        public decimal diamondBidRate { get; set; }//Convert.ToDecimal(Math.Round((Convert.ToDecimal(diamondBid) / Convert.ToDecimal(diamondPrice)) * 100, 2)).ToString() + "%";
        public decimal limitedBidRate { get; set; }// Convert.ToDecimal(Math.Round((Convert.ToDecimal(limitedBid) / Convert.ToDecimal(limitedPrice)) * 100, 2)).ToString() + "%";
        public decimal limitedVipBidRate { get; set; }// Convert.ToDecimal(Math.Round((Convert.ToDecimal(limitedVipBid) / Convert.ToDecimal(limitedVipPrice)) * 100, 2)).ToString() + "%";

        public short IsLimitedOutlet { get; set; }
        public int OrderFlag { get; set; }
        public string TopicProductNo { get; set; }

        public int Sort { get; set; }//排序值

        public int Id { get; set; }//关系表的ID

        /// <summary>
        /// Sku促销总数
        /// </summary>
        public int PromotionCount { get; set; }
        //明天从这里开始，确定是否要加关系表主键
        //------------专题商品扩展------------//

        public string ProductModel { get; set; }//商品货号
        public string ProductShowPic { get; set; }//商品主图
        public decimal GoldPrice { get; set; }//黄金价
        public decimal StandardPrice { get; set; }//尚品价
        public int PcSaleState { get; set; }//PC端售卖状态

        public decimal MinSellPrice { get; set; }//最小黄金价

        public decimal MinPlatinumPrice { get; set; }//最小白金价

        public decimal MinDiamondPrice { get; set; }//最小钻石价

        public decimal MinLimitedPrice { get; set; }//最小尚品价格

        public decimal MinMarketPrice { get; set; }//最小市场价

        public decimal MinPromotionPrice { get; set; }//最小促销价格

        public decimal MaxSellPrice { get; set; }//最大黄金价

        public decimal MaxPlatinumPrice { get; set; }//最大白金价

        public decimal MaxDiamondPrice { get; set; }//最大钻石价

        public decimal MaxLimitedPrice { get; set; }//最大尚品价格

        public decimal MaxMarketPrice { get; set; }//最大市场价

        public decimal MaxPromotionPrice { get; set; }//最大促销价格
        public int ProductSex { get; set; }//男女款
        public int TypeFlag { get; set; }//商品所属
        public int PcShowState { get; set; }//PC端是否显示
        public string ProductShowFlag { get; set; }//新品标识
        public decimal DiscountShangpin { get; set; }//商品在尚品的折扣

        public string MarketPriceRegion { get; set; }//市场区间价
        public string StandardPriceRegion { get; set; }//尚品区间价
        public string PlatinumPriceRegion { get; set; }//白金区间价
        public string DiamondPriceRegion { get; set; }//钻石区间价
        public string PromotionPriceRegion { get; set; }//促销区间价
        public string GoldPriceRegion { get; set; }//黄金区间价
        public int IsOutSide { get; set; }//境内 或者 境外产品
        public string SkuNo { get; set; }
    }
}
