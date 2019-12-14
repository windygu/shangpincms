using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Orders
{
    public class AlipayAddressResultInfo
    {
        public string Type { get; set; }
        public string ProvinceId { get; set; }
        public string CityId { get; set; }
        public string AreaId { get; set; }
        public string Sign { get; set; }

        public string Address { get; set; }
        public string FullName { get; set; }
        public string AddressCode { get; set; }
        public string Area { get; set; }
        public string MobilePhone { get; set; }
        public string Post { get; set; }
        public string City { get; set; }
        public string Prov { get; set; }
        public string Phone { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Phone3 { get; set; }
        public string IsShow { get; set; }
    }
}
