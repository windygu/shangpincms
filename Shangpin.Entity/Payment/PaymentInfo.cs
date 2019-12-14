namespace Shangpin.Entity.Payment
{
    public class PaymentInfo
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; } 
        /// <summary>
        /// 支付ID号//支付宝使用
        /// </summary>
        public string PayTypeChildId{ get; set; }
        /// <summary>
        /// 支付类型，15银行直连 
        /// </summary>
        public string PayTypeId{ get; set; }
        /// <summary>
        /// 支付ID号//支付宝使用 bankid
        /// </summary>
        public string BankId{ get; set; }   
        /// <summary>
        /// 支付ID号//支付宝使用 directPay ,bankPay,motoPay
        /// </summary>
        public string PayMethod { get; set; }      
        
        /// <summary>
        /// 页面支付提交时使用的名称
        /// </summary>
        public string  BankName { get; set; } 
    }
}
