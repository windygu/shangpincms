using System;

namespace Shangpin.Entity.Payment
{
    public class EGiftCardInfo
    {
        public string EGiftCardNo { get; set; }
        public string PassWord { get; set; }
        public string ReciveName { get; set; }
        public string FromName { get; set; }
        public string PresentInfo { get; set; }
        public DateTime DateEnd { get; set; }
        public decimal Amount { get; set; }

        public string OrderNo { get; set; }
        public string MobileNo { get; set; }
        public string CategoryNo { get; set; }
    }
}
