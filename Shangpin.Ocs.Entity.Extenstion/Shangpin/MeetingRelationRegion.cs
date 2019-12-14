using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class MeetingRelationRegion
    {
        public int MeetingRelationID { get; set; }
        public string ImgNO { get; set; }
        public int RegionID { get; set; }
        public short RelationType { get; set; }
        public string Direction { get; set; }
        public string RelationContent { get; set; }
        public int AboutID { get; set; }
    }
}
