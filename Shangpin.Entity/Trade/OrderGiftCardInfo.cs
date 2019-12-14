using System;

namespace Shangpin.Entity.Trade
{
    public class OrderGiftCardInfo
    {
        /// <summary>
        /// 接受祝福的人
        /// </summary>
        public string ReciveName { get; set; }
        /// <summary>
        /// 祝福信息
        /// </summary>
        public string PresentInfo { get; set; }
        /// <summary>
        /// 礼品卡编号
        /// </summary>
        public string EGiftCardNo { get; set; }
        /// <summary>
        /// 面值
        /// </summary>
        public decimal EGiftCardAmount { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime DateEnd { get; set; }
        /// <summary>
        /// 有效期字符串类型
        /// </summary>
        public string DateEndStr { get; set; }
        public string MobileNo { get; set; }
        public int SendCount { get; set; }
    }
}
