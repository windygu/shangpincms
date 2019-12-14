using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Common;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    public class ProductInfo : PagingEntityBase
    {
        public string ProductNo { get; set; }
        public string GoodsNo { get; set; }
        public string ProductName { get; set; }
        public string ProductPicFile { get; set; }

        /// <summary>
        /// 分组编号
        /// </summary>
        public string CategoryNo { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string Sort { get; set; }


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

        /// <summary>
        /// 最大市场价
        /// </summary>
        public decimal MaxMarketPrice { get; set; }
        /// <summary>
        /// 最大奥莱价
        /// </summary>
        public decimal MaxOutletPrice { get; set; }
        /// <summary>
        /// 最大黄金价
        /// </summary>
        public decimal MaxGoldPrice { get; set; }
        /// <summary>
        /// 最大白金价
        /// </summary>
        public decimal MaxPlatinumPrice { get; set; }
        /// <summary>
        /// 最大钻石会员价
        /// </summary>
        public decimal MaxDiamondPrice { get; set; }
        /// <summary>
        /// 最大促销价
        /// </summary>
        public decimal MaxPromotionPrice { get; set; }
        /// <summary>
        /// 最大尚品价
        /// </summary>
        public decimal MaxStandardPrice { get; set; }
        /// <summary>
        /// 最小市场价
        /// </summary>
        public decimal MinMarketPrice { get; set; }
        /// <summary>
        /// 最小奥莱价
        /// </summary>
        public decimal MinOutletPrice { get; set; }
        /// <summary>
        /// 最小黄金价
        /// </summary>
        public decimal MinGoldPrice { get; set; }
        /// <summary>
        /// 最小白金会员价
        /// </summary>
        public decimal MinPlatinumPrice { get; set; }
        /// <summary>
        /// 最小钻石会员价
        /// </summary>
        public decimal MinDiamondPrice { get; set; }
        /// <summary>
        /// 最小促销价
        /// </summary>
        public decimal MinPromotionPrice { get; set; }
        /// <summary>
        /// 最小尚品价
        /// </summary>
        public string BrandNo { get; set; }
        public string BrandEnName { get; set; }
        public string BrandCnName { get; set; }
        public int Quantity { get; set; }
        public int LockQuantity { get; set; }
        public int Status { get; set; }
        //库龄
        public int InventoryAge { get; set; }

        public short IsShelf { get; set; }
        /// <summary>
        /// 商品所属部门
        /// </summary>
        public string DepartmentNo { get; set; }

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


        //扩展字段
        public int SortQuantity { get; set; }


        public int sortvalue { get; set; }
        //明天从这里开始，确定是否要加关系表主键
        //------------专题商品扩展------------//

        public string ProductModel { get; set; }//商品货号
        public string ProductShowPic { get; set; }//商品主图
        public decimal GoldPrice { get; set; }//黄金价
        public decimal StandardPrice { get; set; }//尚品价
        public int PcSaleState { get; set; }//PC端售卖状态
        public int ProductSex { get; set; }//男女款
        public int TypeFlag { get; set; }//商品所属
        public int PcShowState { get; set; }//PC端是否显示
        public string ProductShowFlag { get; set; }//新品标识
        public decimal DiscountShangpin { get; set; }//商品在尚品的折扣
        /// <summary>
        /// 市场区间价
        /// </summary>
        public string MarketPriceRegion { get; set; }//市场区间价
        /// <summary>
        /// 尚品区间价
        /// </summary>
        public string StandardPriceRegion { get; set; }//尚品区间价
        /// <summary>
        /// 白金区间价
        /// </summary>
        public string PlatinumPriceRegion { get; set; }//白金区间价
        /// <summary>
        /// 钻石区间价
        /// </summary>
        public string DiamondPriceRegion { get; set; }//钻石区间价
        /// <summary>
        /// 促销区间价
        /// </summary>
        public string PromotionPriceRegion { get; set; }//促销区间价
        /// <summary>
        /// 黄金区间价
        /// </summary>
        public string GoldPriceRegion { get; set; }//黄金区间价
        /// <summary>
        /// 奥莱区间价
        /// </summary>
        public string OutletPriceRegion { get; set; }

        /// <summary>
        /// 冻结状态：0、未冻结    1、已冻结  by lijibo 20141003
        /// </summary>
        public short DisabledState { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public decimal DiscountRate { get; set; }

        /// <summary>
        /// pc 端上架时间
        /// </summary>
        public DateTime DateShelf { get; set; }
    }
}
