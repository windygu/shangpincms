using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Outlet
{
    /// <summary>
    /// 活动信息
    /// </summary>
    [Serializable]
    public class SubjectInfo
    {
        public string SubjectNo { get; set; }
        public string SubjectName { get; set; }
        public string EditWord { get; set; }
        public string Description { get; set; }
        public string BrandContent { get; set; }
        public string TitlePic1 { get; set; }
        public string TitlePic2 { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime DateCreate { get; set; }
        public string BelongsSubjectPic { get; set; }
        public int VisitCount { get; set; }
        public short DiscountType { get; set; }
        public string DiscountTypeValue { get; set; }
        public string FlapPic { get; set; }
        public short ContentIntorType { get; set; }
        public string ContentIntroduction { get; set; }
        public short ContentRecommType { get; set; }
        public string ContentRecommended { get; set; }
        public short SubjectTemplate { get; set; }
        public short Gender { get; set; }
        public int Status { get; set; }
        public short SubjectType { get; set; }
        public string SpreadPicture { get; set; }
    }
}
