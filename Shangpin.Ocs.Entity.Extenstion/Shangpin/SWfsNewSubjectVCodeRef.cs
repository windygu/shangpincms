using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Common;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class SWfsNewSubjectVCodeRef  
    {
        public string SubjectName { get; set; }
        public string SubjectNo { get; set; } 
        public string SubjectPreName { get; set; }
        public short Status { get; set; }
        public int ActivityTopicsId { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime DatePre { get; set; }

    }
}
