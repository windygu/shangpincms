using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.BDActivity
{
    public class BDActivityInfo
    {
        /// <summary>
        /// 活动号
        /// </summary>
        public string ActivityNo { get; set; }
        /// <summary>
        /// 获取代金券号
        /// </summary>
        public string ChargeCodeNo { get; set; }
        /// <summary>
        /// 邀请码
        /// </summary>
        public string InviteCode { get; set; }
        /// <summary>
        /// 跳转连接地址
        /// </summary>
        public string ActivityUrl { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime DateStart { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime DateEnd { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否注册立刻发送代金券  -- zxj 2012.10.23   val:0|1
        /// </summary>
        public string IsRegSendVouchers { get; set; }

        /// <summary>
        /// 是否注册立刻发送通知  -- zxj 2012.10.23   val:0|1
        /// </summary>
        public string IsRegSendNotice { get; set; }

        /// <summary>
        /// 邀请码对应的通知标题  -- zxj 2012.10.23 
        /// </summary>
        public string NoticeTle { get; set; }

        /// <summary>
        /// 邀请码对应的通知内容  -- zxj 2012.10.23 
        /// </summary>
        public string NoticeCon { get; set; }
    }
}
