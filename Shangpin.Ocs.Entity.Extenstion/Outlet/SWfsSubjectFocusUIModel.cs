using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    public class SWfsSubjectFocusUIModel
    {
        public string ID { get; set; }
        public string SubjectNo { get; set; }
        public string SubjectName { get; set; }
        public DateTime ShowDate { get; set; }
        public int Sort { get; set; }
        public string BrandNo { get; set; }
        public string BrandCnName { get; set; }
        public string BrandEnName { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
        public short Status { get; set; }
        public short IsAudited { get; set; }

        public short Type { get; set; }
    }
}
