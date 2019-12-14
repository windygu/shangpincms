using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Trade
{
    public class ProductGroupInfo
    {
        public string productNo { get; set; }
        public decimal GroupPrice { get; set; }
        public string PromotionDeviceNo { get; set; }
        public string PromotionDeviceName { get; set; }
        public short PromotionType { get; set; }
        public bool IsUseTicket { get; set; }
        public string PromotionGroupNo { get; set; }
        public decimal GroupAmount { get; set; }
        public int SortValue { get; set; }
    }
}
