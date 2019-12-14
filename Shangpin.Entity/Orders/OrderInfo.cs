using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Trade;
using Shangpin.Entity.Common;

namespace Shangpin.Entity.Orders
{
    public class OrderInfo:PagingEntityBase
    {
        public string OrderNo { get; set; }
        public decimal PayAmount { get; set; }
        public decimal TicketAmount { get; set; }
        public decimal Freight { get; set; }
        public short Status { get; set; }
        public short PayTypeId { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateCancel { get; set; }
        public DateTime DateComplete { get; set; }
        public DateTime DatePay { get; set; }
        public string UserId { get; set; }
        public short PayStatus { get; set; }
        public short LockStatus { get; set; }
        public short IsTradeOrder { get; set; }
        public short OrderStyle { get; set; }
        public short IsUseTicket { get; set; }
        public string StatusName { get; set; }
        public string PaySatusName { get; set; }
        public bool IsShowPay { get; set; }
        public bool IsShowCancel { get; set; }
        public bool IsShowConfirmGoods { get; set; }
        public int OrderPayTimeSpan { get; set; }
        public string OrderStep { get; set; }
        public string ConsigneeName { get; set; }
        public string ConsigneeProvinceName { get; set; }
        public string ConsigneeCityName { get; set; }
        public string ConsigneeAreaName { get; set; }
        public string ConsigneeAddress { get; set; }
        public string ConsigneePostCode { get; set; }
        public string ConsigneeMobile { get; set; }
        public string ConsigneePhone { get; set; }
        public string DeliverName { get; set; }
        public string PayTypeName { get; set; }
        public string InvoiceTitle { get; set; }
        public string LibraryFlagName { get; set; }//物流状态
        public decimal TotalAmount { get; set; }
        public int DeliverDateType { get; set; }
        public List<OrderDetail> OrderDetail { get; set; }

        public List<Group> OrderGroup { get; set; }

        //是否使用礼品卡支付
        public int IsUseGiftCardPay { get; set; }
        //礼品卡是否支付成功
        public int GiftCardPayStatus { get; set; }
        //礼品卡金额
        public decimal GiftCardAmount { get; set; }
        //在线支付金额
        public decimal OnlineAmount { get; set; }

        public bool IsElectronCard { get; set; }

        //物流信息
        public int LogisticsCount { get; set; }
    }

    public class OrderDetail
    {
        public string OrderNo { get; set; }
        public string ProductNo { get; set; }
        public string SkuNo { get; set; }
        public decimal MarketPrice { get; set; }
        public short DeliverStatus { get; set; }
        public decimal SellPrice { get; set; }
        public int Quantity { get; set; }
        public string SkuAttrText { get; set; }
        public string ProductName { get; set; }
        public string ProductPicFile { get; set; }
        public string CategoryNo { get; set; }
        public string SkuHashText { get; set; }
        public short SizeStandard { get; set; }
        public string pCategory { get; set; }
        public string BrandCnName { get; set; }
        public string BrandEnName { get; set; }
        public string ProductUrl { get; set; }
        public string NewSize { get; set; }
        public string SizeStandardName { get; set; }
        public string PromotionNo { get; set; }
        public int PromotionProductType { get; set; }
        public int PromotionType { get; set; }
        public decimal? Price { get; set; }
        public int IsSupportCod { get; set; }
        public short TopicSubjectFlag { get; set; }
        public decimal UnitPrice { get; set; }
        public string SubjectNo { get; set; }

        public string OrderDetailNo { get; set; }
        public string GroupNo { get; set; }
        //是否能退
        public bool IsReturn { get; set; }
        //是否能换
        public bool IsChange { get; set; }
        //是否显示退货按钮
        public bool IsShowReturn { get; set; }
        //是否显示换货按钮
        public bool IsShowChange { get; set; }
        public string ReturnChangeFlag { get; set; }
    }

    public class OrderLevelInfo
    {
        public string OrderNo { get; set; }
        public decimal PayAmount { get; set; }
        public short Status { get; set; }
        public DateTime DateComplete { get; set; }
        public string UserId { get; set; }
        public short Type { get; set; }
        public string OrderReturnNo { get; set; }
        public DateTime ReturnDateComplete { get; set; }
        public short ListingOutletFlag { get; set; }
    }
    public class OrderAmountInfo
    {
        public decimal TotalAmount { get; set; }
        public decimal OutletTotalAmount { get; set; }
        public decimal ShangTotalAmount { get; set; }

        public decimal YearAmount { get; set; }
        public decimal OutletYearAmount { get; set; }
        public decimal ShangYearAmount { get; set; }
    }

    public class NewOrderDetail
    {
        public string OrderNo { get; set; }
        public string ProductNo { get; set; }
        public string SkuNo { get; set; }
        public decimal MarketPrice { get; set; }
        public short DeliverStatus { get; set; }
        public decimal SellPrice { get; set; }
        public int Quantity { get; set; }
        public string SkuAttrText { get; set; }
        public string ProductName { get; set; }
        public string ProductPicFile { get; set; }
        public string CategoryNo { get; set; }
        public string SkuHashText { get; set; }
        public short SizeStandard { get; set; }
        public string pCategory { get; set; }
        public string BrandCnName { get; set; }
        public string BrandEnName { get; set; }
        public string ProductUrl { get; set; }
        public string NewSize { get; set; }
        public string SizeStandardName { get; set; }
        public string PromotionNo { get; set; }
        public int PromotionProductType { get; set; }
        public int PromotionType { get; set; }
        public decimal? Price { get; set; }
        public int IsSupportCod { get; set; }
        public short TopicSubjectFlag { get; set; }
        public decimal UnitPrice { get; set; }
        public string SubjectNo { get; set; }

        public string GroupNo { get; set; }
        public string OrderDetailNo { get; set; }

    }

    public class NewMobileOrderDetail
    {
        public string OrderNo { get; set; }
        public string ProductNo { get; set; }
        public string SkuNo { get; set; }
        public decimal MarketPrice { get; set; }
        public short DeliverStatus { get; set; }
        public decimal SellPrice { get; set; }
        public int Quantity { get; set; }
        public string SkuAttrText { get; set; }
        public string ProductName { get; set; }
        public string ProductPicFile { get; set; }
        public string CategoryNo { get; set; }
        public string SkuHashText { get; set; }
        public short SizeStandard { get; set; }
        public string pCategory { get; set; }
        public string BrandCnName { get; set; }
        public string BrandEnName { get; set; }
        public string ProductUrl { get; set; }
        public string NewSize { get; set; }
        public string SizeStandardName { get; set; }
        public string PromotionNo { get; set; }
        public int PromotionProductType { get; set; }
        public int PromotionType { get; set; }
        public decimal? Price { get; set; }
        public int IsSupportCod { get; set; }
        public short TopicSubjectFlag { get; set; }
        public decimal UnitPrice { get; set; }
        public string SubjectNo { get; set; }
        public string OrderDetailNo { get; set; }
        public string GroupNo { get; set; }
        public string ReturnChangeFlag { get; set; }

    }
}
