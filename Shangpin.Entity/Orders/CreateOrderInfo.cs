using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Orders
{
    public class CreateOrderInfo
    {
        //验证方式 默认0
        public string AjaxPost { get; set; }
        //是否使用优惠券1使用0未使用
        public string CashTicketFlag { get; set; }
        //标记是否开发票 1 使用 0 未使用
        public string InvoiceFlag { get; set; }
        //优惠券编号
        public string CashTicketNo { get; set; }
        //发票抬头类型1个人0 单位
        public string InvoiceType { get; set; }
        //发票抬头信息
        public string InvoiceTitle { get; set; }
        //发票内容
        public string InvoiceContent { get; set; }
        //配送方式
        public string TimeRequest { get; set; }
        //public string SpecialRequest { get; set; }
        //发票地址id
        public string InvoiceAddressId { get; set; }
        //收货地址id
        public string ConsigneeId { get; set; }
        //购买明细id
        public string BuyDetailIds { get; set; }

        public string payTypeId { get; set; }
        public string payTypeChildId { get; set; }

        public string DeliverRequest { get; set; }

        //实物礼品卡
        //1：礼品卡电子卡（电子卡）2：礼品卡刮刮卡（实物卡）3：礼品卡磁条卡（实物卡） 
        public short CardType { get; set; }
        //购买类型：0普通购买(所有普通商品) 1立即购买(礼品卡实物卡)
        public short BuyType { get; set; }
        public string BuyKey { get; set; }
        /// <summary>
        /// 优惠券订单使用
        /// </summary>
        public string Mobile { get; set; }
    }
}
