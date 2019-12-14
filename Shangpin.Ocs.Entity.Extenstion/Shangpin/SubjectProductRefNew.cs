using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Common;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class SubjectProductRefNew : PagingEntityBase
    {
        public string SubjectProductRefId { get; set; }
        public int PropertyValue { get; set; }
        public int TopFlag { get; set; }
        public string SortNo { get; set; }
        public string BuyType { get; set; }
        public short IsStarProduct { get; set; }
        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public string ProductPicFile { get; set; }
        public string GoodsNo { get; set; }
        public decimal MarketPrice { get; set; }
        public decimal SellPrice { get; set; }
        public decimal PlatinumPrice { get; set; }
        public decimal DiamondPrice { get; set; }
        public decimal LimitedPrice { get; set; }
        public decimal LimitedVipPrice { get; set; }
        public short IsShelf { get; set; }
        public string BrandNo { get; set; }
        public string BrandEnName { get; set; }
        public string BrandCnName { get; set; }
        public int Quantity { get; set; }
        public int LockQuantity { get; set; }
        public decimal DiscountRate { get; set; }
        public DateTime ShowTime { get; set; }
        public bool IsShow { get; set; }
        public decimal PromotionPrice { get; set; }

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

        public string MarketPriceRegion { get; set; }//市场区间价
        public string StandardPriceRegion { get; set; }//尚品区间价
        public string PlatinumPriceRegion { get; set; }//白金区间价
        public string DiamondPriceRegion { get; set; }//钻石区间价
        public string PromotionPriceRegion { get; set; }//促销区间价
        public string GoldPriceRegion { get; set; }//黄金区间价
        public int IsOutSide { get; set; }//是否是境内外商品
    }
}

