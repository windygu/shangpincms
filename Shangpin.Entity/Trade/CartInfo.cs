using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Trade
{
    public class CartInfo
        {
            //购物车信息
            public string ShoppingCartDetailId { get; set; }
            public string ProductNo { get; set; }
            public string SkuNo { get; set; }
            public string CategoryNo { get; set; }
            public int Quantity { get; set; }
            public DateTime DateAdd { get; set; }
            public decimal Price { get; set; }
            public decimal TotalQuantityAmount { get; set; }
            public decimal TotalAmount { get; set; }
            public int TotalQuantity { get; set; }
            public string CartType { get; set; }
            //1:售罄 2：剩余量 3：活动过期 4:商品已下架
            public int MsgType { get; set; }
            public string Msg { get; set; }
            public string SubjectDateEnd { get; set; }
            public int InventoryQuantity { get; set; }
            //商品信息
            public string ProductName { get; set; }
            public string BrandEnName { get; set; }
            public int IsSupportDiscount { get; set; }
            //public string IsSupportDiscountFlag { get; set; }
            public string ProductPicFile { get; set; }
            public string ProductCategoryNo { get; set; }
            //sku信息
            //public decimal PlatinumPrice { get; set; }
            //public decimal DiamondPrice { get; set; }
            //public decimal LimitedPrice { get; set; }
            //public decimal LimitedVipPrice { get; set; }
            public string SkuAttrText { get; set; }
            public string SkuHashText { get; set; }
            public string SkuSize { get; set; }
            public string SizeName { get; set; }
            public string SizeStandardName { get; set; }
            public string ProductUrl { get; set; }
            public short TopicSubjectFlag { get; set; }
            public decimal FavoritePrice { get; set; }
            //促销组合信息
            public string GId { get; set; }
            public decimal GroupPrice { get; set; }
            public string PromotionDeviceNo { get; set; }
            public int IsUserTicket { get; set; }
            public string PromotionGroupNo { get; set; }
            public int PromotionType { get; set; }
            public string GroupId { get; set; }

            public int IsSupportCod { get; set; }
            public short IsPromotion { get; set; }
            //实物礼品卡
            //1：礼品卡电子卡（电子卡）2：礼品卡刮刮卡（实物卡）3：礼品卡磁条卡（实物卡） 
            public short CardType { get; set; }
            //购买类型：0普通购买(所有普通商品) 1立即购买(礼品卡实物卡)
            public short BuyType { get; set; }

            public string VipNo { get; set; }
        }
}
