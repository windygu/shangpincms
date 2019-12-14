using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Common;

namespace Shangpin.Entity.User
{
    public partial class WfsConsigneeX:PagingEntityBase
    {

        public Int32 ConsigneeId { get; set; }

        public String UserId { get; set; }

        public String ConsigneeName { get; set; }

        public String ConsigneePhone { get; set; }

        public String ConsigneeMobile { get; set; }

        public String ConsigneeEmail { get; set; }

        public Int32 ConsigneeProvinceId { get; set; }

        public Int32 ConsigneeCityId { get; set; }

        public Int32 ConsigneeAreaId { get; set; }

        public String ConsigneeAddress { get; set; }

        public String ConsigneePostCode { get; set; }

        public DateTime DateCreate { get; set; }

        public Int16 LastFlag { get; set; }

        public String Memo { get; set; }

        public Int16 IsInvoice { get; set; }
    }

}
