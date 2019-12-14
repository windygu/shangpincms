using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin.ProductFlat
{
    public class ProductHot
    {
        public string ProductNo { get; set; }//商品编号
        public long ProductHotValue { get; set; }//商品热度值
        public long ProductSevenHotValue { get; set; }//商品7日热度值
    }
}
