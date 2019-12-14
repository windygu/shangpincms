using Shangpin.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    public class ChannelFeaturedEventsInfo : PagingEntityBase
    {
        public int ID { get; set; }
        public string SubjectNo { get; set; }
        public string SubjectName { get; set; }
        public string ChannelNo { get; set; }
        public int Location { get; set; }
        public int Type { get; set; } 
        public DateTime DateShow { get; set; }
        public string SpreadPicture { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime DateCreate { get; set; }
        public string CreateUserId { get; set; }
    }
}
