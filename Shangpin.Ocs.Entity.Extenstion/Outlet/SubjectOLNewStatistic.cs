using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Entity.Wfs;
using System.Xml.Serialization;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    public class SubjectOLNewStatistic
    {
        /// <summary>
        /// 活动列表
        /// </summary>
        public SubjectInfo NewSubject { get; set; }
        /// <summary>
        /// 销售统计
        /// </summary>
        public SubjectSaleStatistic SaleStatistic { get; set; }
        /// <summary>
        /// 订单PV会员统计
        /// </summary>
        public SubjectVisitStatistic VisitStatistic { get; set; }
        /// <summary>
        /// 活动所属频道
        /// </summary>
        public IList<SWfsChannel> ChannelList { get; set; }
        /// <summary>
        /// 活动所属品类
        /// </summary>
        public IList<SWfsSubjectCategoryRef> CategoryRefList { get; set; }
    }

    public class SubjectOLNewStatisticInfo
    {
        /// <summary>
        /// 活动列表
        /// </summary>
        public IList<SubjectOLNewStatistic> SubjectNewStatisticList { get; set; }
        /// <summary>
        /// 所属品类
        /// </summary>
        public IList<SpCategory> ErpCategoryList { get; set; }
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

    public class SubjectSaleVisitStatisticsDataM
    {
        /// <summary>
        /// 活动编号
        /// </summary>
        public string SubjectNo { get; set; }

        /// <summary>
        /// 活动信息
        /// </summary>
        public SubjectInfo Subject { get; set; }

        /// <summary>
        /// 销售统计
        /// </summary>
        public SubjectSaleStatistic SaleStatistic { get; set; }

        /// <summary>
        /// 访问统计
        /// </summary>
        public SubjectVisitStatistic VisitStatistic { get; set; }
    }

    [XmlRoot("XML")]
    [Serializable]
    public class SubjectSaleVisitStatisticsDataModel
    {
        public SubjectSaleVisitStatisticsDataModel() { }

        /// <summary>
        /// 活动编号
        /// </summary>

        [XmlElement("SubjectNo")]
        public string SubjectNo { get; set; }

        /// <summary>
        /// 销售统计
        /// </summary>
        [XmlElement("SaleStatistic")]
        public SubjectSaleStatistic SaleStatistic { get; set; }

        /// <summary>
        /// 访问统计
        /// </summary>
        [XmlElement("VisitStatistic")]
        public SubjectVisitStatistic VisitStatistic { get; set; }
    }
}
