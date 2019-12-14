using System;

namespace Shangpin.Entity.Trade
{
    public class PaymentOrderDetailListInfo
    {
        public String ProductName { get; set; }
        public String ProductNo { get; set; }
        public String SkuNo { get; set; }
        public int Quantity { get; set; }
        public string ProductPicFile { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal MarkPrice { get; set; }
        public string SkuAttrText { get; set; }
    } 
}
