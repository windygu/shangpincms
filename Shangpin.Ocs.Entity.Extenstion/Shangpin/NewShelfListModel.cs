using Shangpin.Entity.Wfs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class NewShelfListModel
    {
        public SwfsFlagShipNewArrival Arrival { get; set; }

        public List<ProductInfoNew> OneRowProductList { get; set; }//第一行的商品信息

        public List<ProductInfoNew> TwoRowProductList { get; set; }//第二行的商品信息
    }
}
