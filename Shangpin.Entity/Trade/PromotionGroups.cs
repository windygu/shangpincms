using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Trade
{
    public class PromotionGroups
    {
        public List<Group> Groups { get; set; }
    }

    public class Group
    {
        public List<ProductItem> Items { get; set; }
    }
    [Serializable()]
    public class ProductItem
    {
        public string GID { get; set; }//标识商品序号
        public string GroupID { get; set; }//标识商品所在组合序号
        public string ProductNo { get; set; }
        public string SkuNo { get; set; }
        public int Quantity { get; set; }
        public string CategoryNo { get; set; }
        public string VipNo { get; set; }
        public Nullable<System.DateTime> DateAdd { get; set; }
        public decimal Price { get; set; }//单价
        public decimal GroupPrice { get; set; }//组合单价
        public string PromotionDeviceNo { get; set; }
        public string PromotionDeviceName { get; set; }
        public short PromotionType { get; set; }
        public int IsUserTicket { get; set; }
        public string PromotionGroupNo { get; set; }
        public string operateFlag { get; set; }//增删改标识

        public string CartType { get; set; }
        //1:售罄 2：剩余量 3：活动过期 4:商品已下架
        public int MsgType { get; set; }
        public string Msg { get; set; }
        public int InventoryQuantity { get; set; }
        //商品信息
        public string ProductName { get; set; }
        public string BrandEnName { get; set; }
        public int IsSupportDiscount { get; set; }
        public string ProductPicFile { get; set; }
        //sku信息       
        public string SkuAttrText { get; set; }
        public string SkuHashText { get; set; }
        public string NewSize { get; set; }
        public int IsSupportCod { get; set; }
        public decimal UnitPrice { get; set; }
        public string SubjectNo { get; set; }
        public string SkuSize { get; set; }
        public string SizeName { get; set; }
        public string SizeStandardName { get; set; }
        public string ProductUrl { get; set; }
        public short TopicSubjectFlag { get; set; }
        public decimal FavoritePrice { get; set; }
        public string ShoppingCartDetailId { get; set; }
        public string ProductCategoryNo { get; set; }
        public decimal TotalPrice { get; set; }//单价
        public decimal TotalGroupPrice { get; set; }//组合单价
        public string pCategory { get; set; }
        public string BrandCnName { get; set; }
        public short SizeStandard { get; set; }
        public decimal SellPrice { get; set; }
        public string OrderNo { get; set; }
        public int IsDisabledGroup { get; set; }
        public string SubjectDateEnd { get; set; }

        //奥莱订单显示新增字段 ljx
        public string OrderDetailNo { get; set; }
        public short IsPromotion { get; set; }

        public decimal MarketPrice { get; set; }
        public short DeliverStatus { get; set; }
        public string PromotionNo { get; set; }
        public int PromotionProductType { get; set; }
        //是否能退
        public bool IsReturn { get; set; }
        //是否能换
        public bool IsChange { get; set; }
        //是否显示退货按钮
        public bool IsShowReturn { get; set; }
        //是否显示换货按钮
        public bool IsShowChange { get; set; }
        public string ReturnChangeFlag { get; set; }
        public string SupplierOrderNo { get; set; }
    }
}
