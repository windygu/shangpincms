using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class HomeSecKill
    {
        public int SecKillId { get; set; }
        public string ChannelNo { get; set; }
        public string SecKillTitle { get; set; }
        public short SecKillType { get; set; }
        public short Status { get; set; }
        public string ProductNo { get; set; }
        public DateTime ShowTime { get; set; }
        public DateTime StartTime { get; set; }
        public string LinkAddress { get; set; }
        public DateTime CreateTime { get; set; }
        public string ProductPicFile { get; set; }
        public string ProductName { get; set; }
        public string BrandEnName { get; set; }
    }
}

