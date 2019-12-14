using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Outlet
{
    public class PortfolioSaleModel
    {
        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public string PictureFileNo { get; set; }
        public decimal LimitedVipPrice { get; set; }
        public string BrandNo { get; set; }
        public string BrandEnName { get; set; }
        public string BrandCnName { get; set; }
        public string DeviceNo { get; set; }
        public string DeviceName { get; set; }
        public string GroupNo { get; set; }
        public decimal GroupAmount { get; set; }
        public int GroupSort { get; set; }
        public int Quantity { get; set; }
        public int IsSpan { get; set; }
        public short GenderStyle { get; set; }
    }
}
