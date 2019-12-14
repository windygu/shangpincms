using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Trade
{
    [Serializable]
    public class JsonMiniCartModel
    {
        public string count { get; set; }
        public string totalprice { get; set; }
        public IList<list> list { get; set; }
        public string quantity { get; set; }

    }
    [Serializable]
    public class list
    {
        public string id { get; set; }
        public string name { get; set; }
        public string price { get; set; }
        public string favoriteprice { get; set; }
        public string count { get; set; }
        public string brandname { get; set; }
        public string url { get; set; }
        public string img { get; set; }
        public string proNo { get; set; }
        public string skuNo { get; set; }
        public string caNo { get; set; }
        public DateTime datetime { get; set; }
    }
}
