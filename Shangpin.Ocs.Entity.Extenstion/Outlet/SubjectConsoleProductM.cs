using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Common;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    [Serializable]
    public class SubjectConsoleProductM
    {
        public string SubjectNo { get; set; }
        public int ProductNum { get; set; }
    }

    [Serializable]
    public class SubjectUVCountM
    {
        public string SubjectNo { get; set; }
        public int DataCount { get; set; }
    }

    public class WaitingBrandClassM
    {
        public string BrandNo { get; set; }
        public List<string> SordNo { get; set; }
    }
}
