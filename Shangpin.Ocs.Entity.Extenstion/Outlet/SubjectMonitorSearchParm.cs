using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    public class SubjectMonitorSearchParm
    {
        public string SubjectNameNo { get; set; }
        public string BrandNo { get; set; }//插件命名真CD
        public string BrandName { get; set; }//插件命名真CD
        public string ChannelSord { get; set; }
        public string BU { get; set; }
        public string QueryStartTime { get; set; } //查询开始时间
        public string QueryEndTime { get; set; } //查询结束时间

    }
}
