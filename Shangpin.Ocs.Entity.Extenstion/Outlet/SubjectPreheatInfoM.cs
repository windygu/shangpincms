using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Common;
using Shangpin.Entity.Wfs;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    [Serializable]
    public class SubjectPreheatInfoM
    {
        public DateTime PreheatTime { get; set; }
        public string StExpand { get; set; }
        public string SubjectNo { get; set; }
        public DateTime TopCreateTime { get; set; }
        public string PageName { get; set; }
    }
}
