using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.User
{
    public class ApplyOrderStatusInfo
    {
        public string ApplyOrderNo { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string StatusName { get; set; }
    }
}
