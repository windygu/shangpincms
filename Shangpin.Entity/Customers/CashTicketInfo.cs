using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Common;

namespace Shangpin.Entity.Customers
{
    public class CashTicketInfo : PagingEntityBase
    {
        public string CashTicketNo { get; set; }
        public decimal TicketAmount { get; set; }
        public string PromotionTicketName { get; set; }
        public string TicketDesc { get; set; }
        public DateTime DateUseBegin { get; set; }
        public DateTime DateUseEnd { get; set; }
        public string OrderCode { get; set; }
        public DateTime DateUsed { get; set; }
        public DateTime DateDeliver { get; set; }
        public int IsUsed { get; set; }
        public short ConditionType { get; set; }
        public short UsedType { get; set; }
        public string StatusName { get; set; }
        public DateTime PromotionDateStar { get; set; }
        public DateTime PromotionDateEnd { get; set; }

        public string CategoryNos { get; set; }
        public string CategoryNames { get; set; }
        public string BrandNos { get; set; }
        public string BrandNames { get; set; }
        public decimal OrderAmount { get; set; }
        public string SubjectNos { get; set; }
        public string SubjectNames { get; set; }
        public string ProductNos { get; set; }
        public string ProductNames { get; set; }
        public string singleUrl { get; set; }
        public string OrderDetailUrl { get; set; }

        /// <summary>
        /// 策略类型
        /// </summary>
        public int DateType { get; set; }
        /// <summary>
        /// 策略中使用优惠券天数
        /// </summary>
        public int DurationDays { get; set; }
        /// <summary>
        /// 策略创建时间
        /// </summary>
        public DateTime DateCreate { get; set; }
        /// <summary>
        /// 策略状态
        /// </summary>
        public Int16 Status { get; set; }
        /// <summary>
        /// 策略中使用优惠券开始时间
        /// </summary>
        public DateTime DateBegin { get; set; }
        /// <summary>
        /// 策略中使用优惠券结束时间
        /// </summary>
        public DateTime DateEnd { get; set; }
    }


    public class CashTicketAttr
    {
        public DateTime DateUseBegin { get; set; }
        public DateTime DateUseEnd { get; set; }
        public string CashTicketNo { get; set; }
        public decimal TicketAmount { get; set; }
        public int UsedType { get; set; }
        public decimal OrderAmount { get; set; }
    }
}
