using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Customers
{
    public class SWfsCDKInfo
    {
        public string CDKBathNo { set; get; }

        public short CDKStatus { set; get; }

        public int TotalActivationTimes { set; get; }

        public int EnableActivationTimes { set; get; }

        public DateTime DateBegin { set; get; }

        public DateTime DateEnd { set; get; }

        public short CDKBatchStatus { set; get; }
    }
}
