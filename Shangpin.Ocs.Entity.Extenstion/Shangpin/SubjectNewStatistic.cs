using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Entity.Wfs;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class SubjectNewStatistic
    {
        /// <summary>
        /// 活动列表
        /// </summary>
        public SWfsNewSubject NewSubject { get; set; }
        /// <summary>
        /// 销售统计
        /// </summary>
        public SubjectSaleStatistic SaleStatistic { get; set; }
        /// <summary>
        /// 订单PV会员统计
        /// </summary>
        public SubjectVisitStatistic VisitStatistic{ get; set; }
   
        
    }

    public class SubjectNewStatisticInfo
    {
        /// <summary>
        /// 活动列表
        /// </summary>
        public IList<SubjectNewStatistic> SubjectNewStatisticList { get; set; }
        /// <summary>
        /// 所属品类
        /// </summary>
        public IList<ErpCategory> ErpCategoryList { get; set; }
        /// <summary>
        /// 销售统计
        /// </summary>
        public SubjectSaleStatistic SaleStatistic { get; set; }
        /// <summary>
        /// 订单PV会员统计
        /// </summary>
        public SubjectVisitStatistic VisitStatistic { get; set; }
           public string SubjectNos { get; set; }
           public string Range { get; set; }
           public string BeginTime { get; set; }
           public string EndTime { get; set; }
           public string BeginTimeSubject { get; set; }
           public string EndTimeSubject { get; set; }

           public int SubjectCount { get; set; } 
    }
}
