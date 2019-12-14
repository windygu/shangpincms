using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class ActiveAndSpecial
    {
        public int ActiveID { get; set; }
        public string ActiveNO { get; set; }
        public string MainMeetingNO { get; set; }
        public string MeetingNO { get; set; }
        public string BrandNO { get; set; }
        public string ActiveName { get; set; }
        public string ImgNo { get; set; }
        public string BrandLink { get; set; }
        public string ActiveLink { get; set; }
        public DateTime CreateTime { get; set; }
        public int ActiveType { get; set; }//1活动0专题
        public bool IsSelect { get; set; }
        public int Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string Webpic { get; set; }//活动列表页banner图
        public string Mobilepic { get; set; }//IPhone图片

        public int WebSource { get; set; }//网站来源(0尚品、1奥莱)
        public string ActiveStartTime { get; set; }
        public string ActiveEndTime { get; set; }
        
    }
}
