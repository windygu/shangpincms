using Shangpin.Entity.Wfs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    /// <summary>
    /// SWfsMeetingInfo 列表显示
    /// </summary>
    public class SWfsMeetingHtmlInfoList
    {
        public string TemplateType { get; set; }
        //public SWfsMeetingInfo SWfsMeetingInfo { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsSelect { get; set; }
        public string MeetingDomain { get; set; }
        public int MeetingID { get; set; }
        public string MeetingName { get; set; }
        public string MeetingNO { get; set; }
        public string MobilePreViewCode { get; set; }
        public string MobilePreViewNO { get; set; }
        public string MobileShowImg { get; set; }
        public string MobileStartCode { get; set; }
        public string MobileStartNO { get; set; }
        public int ParentID { get; set; }
        public DateTime PreViewTime { get; set; }
        public short SourceFrom { get; set; }
        public DateTime StartTime { get; set; }
        public short Status { get; set; }
        public short WebOrMobile { get; set; }
        public string WebPreViewCode { get; set; }
        public string WebPreViewNO { get; set; }
        public string WebStartCode { get; set; }
        public string WebStartNO { get; set; }
        
    }
}
