using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Sales
{
    public class SaleModel
    {
        public string ProductNo { get; set; }
        public string ProductExtend { get; set; }
        public string ProductName { get; set; }
        public decimal MarketPrice { get; set; }
        public decimal LimitedPrice { get; set; }
        public decimal PromotionPrice { get; set; }
        public decimal DiscountRate { get; set; }
        public string ProductPicFile { get; set; }
        public string CategoryNo { get; set; }
        public string BrandNo { get; set; }
        public string BrandCnName { get; set; }
        public string BrandEnName { get; set; }
        public short Gender { get; set; }
        public int ProductSort { get; set; }
        public short Quantity { get; set; }
        public string RelationNo { get; set; }
        public DateTime DateCreate { get; set; }
        public bool IsShowMarketPrice { get; set; }
        public string ErpCategoryNo { get; set; }
    }
}
