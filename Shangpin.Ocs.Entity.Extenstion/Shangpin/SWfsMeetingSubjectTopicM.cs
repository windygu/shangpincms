using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    /// <summary>
    /// 会场专题和活动实体类
    /// </summary>
    public class SWfsMeetingSubjectTopicM
    {
        public int ActiveID { get; set; }
        public string ActiveLink { get; set; }
        public string ActiveName { get; set; }
        public string ActiveNO { get; set; }
        public string BrandLink { get; set; }
        public string BrandNO { get; set; }
        public string ImgNo { get; set; }
        public short IsActive { get; set; }
        public bool IsSelect { get; set; }
        public string MainMeetingNO { get; set; }
        public string MeetingNO { get; set; }
        public DateTime CreateTime { get; set; }

        public string SubjectName { get; set; }
        public string ChannelNo { get; set; }
        public string BelongsSubjectPic { get; set; }
        public string TitlePic2 { get; set; }

        public string TopicName { get; set; }
        public string SubHeading { get; set; }
        public string TopicURL { get; set; }
        public string TopicPic { get; set; }
        public int Gender { get; set; }

        public short Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
