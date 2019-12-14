using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Customers
{
    public class CDKBatchPromotionStrategy
    {
        public string StrategyNo { set; get; }
        
        public int DateType { set; get; }

        public int DurationDays { set; get; }

        public DateTime DateBegin { set; get; }

        public DateTime DateEnd { set; get; }
    }
}
