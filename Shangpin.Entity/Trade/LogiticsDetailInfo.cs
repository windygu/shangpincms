using System;

namespace Shangpin.Entity.Trade
{
    public class LogiticsDetailInfo
    {
        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime DateOperate { set; get; }

        /// <summary>
        /// 物流动态
        /// </summary>
        public string ScanTitle { set; get; }

        /// <summary>
        /// 物流单号
        /// </summary>
        public string TransportNo { set; get; }

        /// <summary>
        /// 配送状态0：未接收；1：已接受；2：已完成。
        /// </summary>
        public short Status { set; get; }

        /// <summary>
        /// 公司类型 1联邦
        /// </summary>
        public short CompanyType { set; get; }
    }
}
