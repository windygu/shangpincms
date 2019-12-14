using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.User
{
    public class RefundApplyInfo
    {
        public string OrderNo { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string MailAddress { get; set; }
        public string Consignee { get; set; }
        public string Phone { get; set; }
        public string ConsigneeTwo { get; set; }
        public string PhoneTwo { get; set; }
        public string RefundRemarkType { get; set; }
        public string RefundType { get; set; }
        public string OpenAmountBank { get; set; }
        public string OpenAmountNo { get; set; }
        public string OpenAmountUser { get; set; }
        public string SkuNo { get; set; }
        public string PicNo { get; set; }
        public string UserMemo { get; set; }
        public string OrderDetailNo { get; set; }

        public string RefundApplyNo { get; set; }
    }
    public class RefundHistoryInfo
    { 
        public string ReturnApplyOrderNo { get; set; }
		public int Status { get; set; }
		public int Type { get; set; }
        public DateTime ApplyDate { get; set; }
        public IList<RefundHistoryDetail> detail { get; set; }
        public string OrderDetailNo { get; set; }
		
    }
    public class RefundHistoryDetail
    {
        public string ProductNo { get; set; }
		public string SkuNo { get; set; }
		public string SkuAttrText { get; set; }
		public string ProductName { get; set; }
		public string ProductPicFile { get; set; }
		public string SkuHashText { get; set; }
		public string SizeStandard { get; set; }
		public string  pCategory { get; set; }
		public string IsSupportCod { get; set; }
        public string MarketPrice { get; set; }

        public string SubjectNo { get; set; }
        public string SkuSize { get; set; }
        public string SizeName { get; set; }
        public string SizeStandardName { get; set; }
        public string ProductUrl { get; set; }
        public short TopicSubjectFlag { get; set; }
        public string CategoryNo { get; set; }
        
        public string ProductCategoryNo { get; set; }
    }
}
