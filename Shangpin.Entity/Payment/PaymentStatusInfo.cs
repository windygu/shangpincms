using System;
using System.Collections.Generic;
using Shangpin.Entity.Orders;
using Shangpin.Entity.User;
using Shangpin.Entity.Wfs;

namespace Shangpin.Entity.Payment
{
    public class PaymentStatusInfo
    {
        /// <summary>
        /// 订单状态暂时没用
        /// </summary>
        public int OrderSubmitStatus { get; set; }
        /// <summary>
        /// 支付剩余时间
        /// </summary>
        public int OrderPayTimeSpan { get; set; }
        /// <summary>
        /// 我的账户url
        /// </summary>
        public string  MyCustomerUrl { get; set; } 
        /// <summary>
        /// 用户信息
        /// </summary>
        public CustomerInfo Customer { get; set; } 
 
        /// <summary>
        /// 订单信息
        /// </summary>
        public WfsOrder OrderSingle { get; set; } 
          /// <summary>
        /// 订单明细信息
        /// </summary>
        public List<OrderDetail> OrderDetailList { get; set; }
        public Boolean IsElGiftCardOrder { get; set; }
        public Boolean IsSendGiftCardKeySuccess { get; set; }
        public string GiftCardKeyErrorInfo { get; set; }
    }
}
