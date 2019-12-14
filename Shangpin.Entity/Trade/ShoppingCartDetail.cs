using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Trade
{
    public class ShoppingCartDetail
    {

        public string ShoppingCartDetailId { get; set; }
        public string ShoppingCartId { get; set; }
        public string ProductNo { get; set; }
        public string SkuNo { get; set; }
        public int Quantity { get; set; }
        public short DeleteFlag { get; set; }
        public Nullable<System.DateTime> DateAdd { get; set; }
        public string CategoryNo { get; set; }
        public short BuyType { get; set; }
        public string PromotionNo { get; set; }
        public string PromotionName { get; set; }
        public short PromotionProductType { get; set; }
        public short PromotionType { get; set; }
        public string RelationSkuNo { get; set; }
        public string BrandNo { get; set; }
        public string ErpCategoryNo { get; set; }
        public short TopicSubjectFlag { get; set; }
        public string VipNo { get; set; }
    }

    public class ShoppingCartDetails
    {
        public List<ShoppingCartDetail> WfsShoppingCartDetails { get; set; }
    }
}
