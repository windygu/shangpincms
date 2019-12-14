using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Orders
{
    public class AddressInfo
    {
        public int Type { get; set; }
        public string ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string CityId { get; set; }
        public string CityName { get; set; }
        public string AreaId { get; set; }
        public string AreaName { get; set; }
    }
}
