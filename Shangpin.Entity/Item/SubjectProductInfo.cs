using System.Collections.Generic;

namespace Shangpin.Entity.Item
{
    public class SubjectProductInfo
    {
        public string ProductNo { get; set; }   
        public string ProductName { get; set; }
        public string ProductPicFile { get; set; }
        public decimal SellPrice { get; set; }
        public decimal MarketPrice { get; set; }
        public decimal LimitedPrice { get; set; }
        public decimal LimitedVipPrice { get; set; }
        public decimal DiamondPrice { get; set; }
        public decimal PlatinumPrice { get; set; }
        public short PropertyValue { get; set; }
        public string SCategoryNo { get; set; }
        public string AttrSize { get; set; }
        public short SizeStandard { get; set; }
        public int Discount { get; set; }
        public string BrandEnName{get;set;}
        public string BrandCnName { get; set; }
        public string ProductSizeStr{get;set;}
        public string AttributeFlag { get; set; }
        public List<string> ProductSizes { get; set; }
        /// <summary>
        /// 是否明显商品
        /// </summary>
        public bool IsStar { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public int ProductCount { get; set; }


        #region 浏览使用
        /// <summary>
        /// 来源类型0专题，1活动
        /// </summary>
        public string FromType { get; set; }
        /// <summary>
        /// 活动分类或专题编号(注：此处填写活动分类或专题编号)
        /// </summary>
        public string SubjectOrTopicNo { get; set; }

        public short Gender { get; set; }
        public string Href { get; set; }

        public string Src { get; set; }

        public string LimitedVipPriceII { get; set; }

        public string MarketPriceII { get; set; }

        #endregion
    }


    public  class  SkuSizeInfo
    {
        public string ProductNo { get; set; }

        public string AttributeTextTwo { get; set; }
    }
}
