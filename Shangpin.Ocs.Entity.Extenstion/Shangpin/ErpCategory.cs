using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class ErpCategory
    {

        public string CategoryName { get; set; }
        public string CategoryNo { get; set; }
        public short CategoryType { get; set; }
        public DateTime DateCreate { get; set; }
        public decimal DefaultRate { get; set; }
        public short IsBatch { get; set; }
        public int MaxPurchaseCount { get; set; }
        public int MaxStockDay { get; set; }
        public decimal MiniRate { get; set; }
        public int MiniStockCount { get; set; }
        public string ParentNo { get; set; }
        public short VisibleStatus { get; set; }
    }
}
