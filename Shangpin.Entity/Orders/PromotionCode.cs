using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Orders
{
    public class PromotionCode
    {
        //优惠码编号
        public string PromotionCodeNo { get; set; }
        //优惠码优惠类型：1：打折；2：直减
        public short PromotionContentType { get; set; }
        //优惠内容
        public int PromotionContent { get; set; }
        //优惠码关联商品折后价格集合
        public Dictionary<string, decimal> ProductDic { get; set; }
        //符合使用条件的商品skunos
        public string SkuNos { get; set; }
        //优惠金额
        public decimal CouponPrice { get; set; }
        //优惠方式
        public short ConditionStyle { get; set; }
        //以下是优惠条件信息
        public decimal OrderAmount { get; set; }
        public int ProductCount { get; set; }
        public short ConditionType { get; set; }
        public string CategoryNos { get; set; }
        public short BrandType { get; set; }
        public string BrandNos { get; set; }
        public string ProductNos { get; set; }
        public string SubjectNos { get; set; }
        public string SpecialProductNos { get; set; }

        public DateTime DateEnd { get; set; }
    }

    public class PromotionProduct
    {
        public string proNo { get; set; }
        public short isSupportDiscount { get; set; }
        public decimal unitPrice { get; set; }
        public decimal limitedPrice { get; set; }
        public int quantity { get; set; }
    }
}
