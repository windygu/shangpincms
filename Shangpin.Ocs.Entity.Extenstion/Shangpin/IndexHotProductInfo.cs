using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    /// <summary>
    /// 热门商品实体
    /// </summary>
    public class IndexHotProductInfo
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
        public int ID { get; set; }

        public string HotCategoryNo { get; set; }
        public DateTime DateShelf { get; set; }

        public int SortValue { get; set; }
        //明天从这里开始，确定是否要加关系表主键
        //------------专题商品扩展------------//
    }
}
