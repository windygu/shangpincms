using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Trade
{
    public class NoticeStateInfo
    {
        public NoticeStateInfo() { }
        public NoticeStateInfo(string n, string s)
        {
            this.N = n;
            this.S = s;
        }

        public NoticeStateInfo(string n, string s, string t, string i)
        {
            this.N = n;
            this.S = s;
            this.T = t;
            this.I = i;
        }

        /// <summary>
        /// 通知ID
        /// </summary>
        public string N { get; set; }
        /// <summary>
        /// 通知状态（1表示未读，2表示已读）
        /// </summary>
        public string S { get; set; }
        /// <summary>
        /// 通知类型（通知类型：1降价通知、2新品通知、3活动通知、4会员升级通知、5会员降级通知、6获得会员通知、7老用户引导获得会员通知）
        /// </summary>
        public string T { get; set; }
        /// <summary>
        /// 通知时间
        /// </summary>
        public string D { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public string I { get; set; }


        public string Title { get; set; }
        public string NContent { get; set; }
        public string NoticeType { get; set; }
        public string Guid { get; set; }
        public string NoticeState { get; set; }
        public string DateCreate { get; set; }
        public string NoticeUrl { get; set; }

    }
}
