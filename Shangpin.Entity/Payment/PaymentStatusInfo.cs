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
        /// ����״̬��ʱû��
        /// </summary>
        public int OrderSubmitStatus { get; set; }
        /// <summary>
        /// ֧��ʣ��ʱ��
        /// </summary>
        public int OrderPayTimeSpan { get; set; }
        /// <summary>
        /// �ҵ��˻�url
        /// </summary>
        public string  MyCustomerUrl { get; set; } 
        /// <summary>
        /// �û���Ϣ
        /// </summary>
        public CustomerInfo Customer { get; set; } 
 
        /// <summary>
        /// ������Ϣ
        /// </summary>
        public WfsOrder OrderSingle { get; set; } 
          /// <summary>
        /// ������ϸ��Ϣ
        /// </summary>
        public List<OrderDetail> OrderDetailList { get; set; }
        public Boolean IsElGiftCardOrder { get; set; }
        public Boolean IsSendGiftCardKeySuccess { get; set; }
        public string GiftCardKeyErrorInfo { get; set; }
    }
}
