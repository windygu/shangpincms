using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    [Serializable]
    public class SkuDto
    {
        public string SkuNo { get; set; }

        public int SkuType { get; set; }

        public StockFlag StockFlag { get; set; }

        public int Quantity { get; set; }

        public string CacheKey { get; set; }

        public bool IsCached { get; set; }

        public string ProductNo { get; set; }

    }
}
