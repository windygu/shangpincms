using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.User
{
    /// <summary>
    /// 换货申请信息
    /// </summary>
    public class RefundReplaceApplyInfo
    {
        /// <summary>
        /// 换货原因编号
        /// </summary>
        public string ReturnReasonDetailNo { get; set; }
        /// <summary>
        /// 换货原因补充问题描述
        /// </summary>
        public string ReturnReasonAppendText { get; set; }
        /// <summary>
        /// 换取的sku编号
        /// </summary>
        public string ChangeSkuNo { get; set; }
        /// <summary>
        /// 换取前的sku编号
        /// </summary>
        public string SkuNo { get; set; }
        /// <summary>
        /// 快递信息编号
        /// </summary>
        public string ConsigneeId { get; set; }
        /// <summary>
        /// 收货时间类型(所有日期均收货，工作日收货，双休日收货)
        /// </summary>
        public string ChangeTimeType { get; set; }
        /// <summary>
        /// 新联系人
        /// </summary>
        public string NewContactPerson { get; set; }
        /// <summary>
        /// 新联系人电话
        /// </summary>
        public string NewContactPersonMobile { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 订单明细编号
        /// </summary>
        public string OrderDetailNo { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
    }
}
