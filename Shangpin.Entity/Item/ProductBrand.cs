using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Item
{
    public class ProductBrand
    {
        public string BrandNo { get; set; }
        public string ProductNo { get; set; }
        public string CategoryNo { get; set; }
        public string ProductName { get; set; }
        public string ProductPicFile { get; set; }
        public short SizeStandard { get; set; }
        public string BrandEnName { get; set; }
        public short IsSupportDiscount { get; set; }
        public string ProductTemplateNo { get; set; }
        public short IsShelf { get; set; }
        public short IsOutletFlag { get; set; }
        public short BrandType { get; set; }
        public short IsPromotion { get; set; }
    }
}
