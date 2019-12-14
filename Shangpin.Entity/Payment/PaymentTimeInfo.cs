namespace Shangpin.Entity.Payment
{
    public class PaymentTimeInfo
    { 
        /// <summary>
        /// 订单错误状态
        /// </summary>
        public int Status { get; set; } 
        /// <summary>
        /// 订单错误信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 订单剩余支付时间
        /// </summary>
        public int RemainSec{ get; set; } 
    }
}
