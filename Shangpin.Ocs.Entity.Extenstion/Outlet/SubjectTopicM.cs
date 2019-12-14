using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    [Serializable]
    public class SubjectTopicM
    {
        public string SubjectNo { get; set; }
        public string SubjectName { get; set; }
        public string SubjectEnName { get; set; }
        public int SubjectTemplate { get; set; }
        public int Gender { get; set; }
        public string AdPic { get; set; }
        public string BelongsSubjectPic { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime TopCreateTime { get; set; }
        public DateTime DateCreate { get; set; }
        public string SWfsAolaiNoticePageId { get; set; }//关联的预热页面编号

        /// <summary>
        /// 自定义链接地址 by lijia 20141020 
        /// （方便运营自定义轮播图链接地址，如会场地址等）
        /// </summary>
        public string CustomUrl { get; set; }

        public short Type { get; set; }

        public string WebUrl { get; set; }

        public string WebPic { get; set; }

        #region 以下是活动的属性，为首页轮播图做筛选用

        public short IsAudited { get; set; }

        public short Status { get; set; }

        public short SubjectType { get; set; }

        public short IsRelated { get; set; }

        public string ChannelNo { get; set; }

        #endregion
    }
}
