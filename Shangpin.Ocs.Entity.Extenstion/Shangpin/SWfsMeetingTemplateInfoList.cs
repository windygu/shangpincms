using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class SWfsMeetingTemplateInfoList
    {
       
        public DateTime CreateTime { get; set; }
        public string CssFileName { get; set; }
        public short isMobile { get; set; }
        public short isPreView { get; set; }
        public string jsFileName { get; set; }
        public string MettingNo { get; set; }
        public string TemplateCode { get; set; }
        public string TemplateDirection { get; set; }
        public int TemplateID { get; set; }
        public string TemplateName { get; set; }
        public SWfsMeetingTemplateInfo SWfsMeetingTemplateInfo { get; set; }
        
    }
}
