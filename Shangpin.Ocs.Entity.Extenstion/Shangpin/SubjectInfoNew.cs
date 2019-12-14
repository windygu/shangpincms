using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Entity.Common;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class SubjectInfoNew : PagingEntityBase
    {
        public string BackgroundPic { get; set; }
        public string BelongsSubjectPic { get; set; }
        public string ChannelNo { get; set; }
        public string ContentIntroduction { get; set; }
        public string ContentRecommended { get; set; }
        public string CreateUserId { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateEnd { get; set; }
        public string Discount { get; set; }
        public short DiscountType { get; set; }
        public string DiscountTypeValue { get; set; }
        public string FlapPic { get; set; }
        public string IPhonePic { get; set; }
        public string IPhoneText { get; set; }
        public short Status { get; set; }
        public string SubjectName { get; set; }
        public string SubjectNo { get; set; }
        public short SubjectTemplate { get; set; }
        public string TitlePic2 { get; set; }
        public bool TopFlag { get; set; }
        public string Hours { get; set; }
        public string Hour { get; set; }
        public string ContentTitleOne { get; set; }
        public string ContentTitleTwo { get; set; }
        /// <summary>
        /// 活动子类的编号
        /// </summary>
        public string CategoryNo { get; set; }
        public IList<SWfsSubjectChannelSordRef> ChannelSordList { get; set; }
        public double TotalHour { get; set; }

        public string SpreadPicture { get; set; }
        public short SpreadStatus { get; set; }
        public int Orderflag { get; set; }
        public short SubjectPreStartTemplateType { get; set; }
        public string PrivilegeValue { get; set; }
    }
}
