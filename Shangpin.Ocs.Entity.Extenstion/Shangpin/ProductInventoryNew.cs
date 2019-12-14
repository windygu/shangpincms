using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class ProductInventoryNew
    {
        public string ProductNo { set; get; }
        public int SumQuantity { set; get; }
        public int SumLockQuantity { set; get; }
        public int Inventory { set; get; }
    }
}
