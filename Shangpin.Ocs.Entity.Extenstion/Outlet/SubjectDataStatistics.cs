using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    public class SubjectDataStatistics
    {
        public IList<SubjectProductStatistic> subjectProductStatistics { get; set; }

        public IList<SubjectSaleStatistic> subjectSaleStatistics { get; set; }

        public IList<SubjectVisitStatistic> subjectVisitStatistic { get; set; }
    }

    public class SubjectProductStatistic
    {
        public int ProductInfoID { get; set; }
        public string SubjectNo { get; set; }
        public DateTime DateStatistics { get; set; }
        public string BrandNo { get; set; }
        public string CategoryNo { get; set; }
        public int SKUCount { get; set; }
        public int StockCount { get; set; }
        public int SaleCount { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public short ListingOutletFlag { get; set; }
        public string BU { get; set; }

    }

    [Serializable]
    public class SubjectSaleStatistic
    {

        public SubjectSaleStatistic() { }

        [XmlElement("SaleInfoID")]
        public int SubjectSaleStatisticsID { get; set; }

        [XmlElement("SubjectNo")]
        public string SubjectNo { get; set; }

        [XmlElement("DateStatistics")]
        public DateTime DateStatistics { get; set; }

        [XmlElement("OrderNums")]
        public decimal OrderNums { get; set; }

        [XmlElement("Amount")]
        public int Amount { get; set; }

        [XmlElement("YesterDayAmount")]
        public int YesterDayAmount { get; set; }

        [XmlElement("CostAmount")]
        public decimal CostAmount { get; set; }

        [XmlElement("StockCount")]
        public int StockCount { get; set; }

        [XmlElement("SKUCount")]
        public int SKUCount { get; set; }

        [XmlElement("SaleCount")]
        public int SaleCount { get; set; }

        [XmlElement("SaledSKUCount")]
        public int SaledSKUCount { get; set; }

        [XmlElement("StockTotalAmount")]
        public decimal StockTotalAmount { get; set; }

        [XmlElement("ListingOutletFlag")]
        public short ListingOutletFlag { get; set; }
    }

    [Serializable]
    public class SubjectVisitStatistic
    {

        public SubjectVisitStatistic() { }

        [XmlElement("VisitinfoID")]
        public int SubjectVisitStatisticsID { get; set; }

        [XmlElement("SubjectNo")]
        public string SubjectNo { get; set; }

        [XmlElement("DateStatistics")]
        public DateTime DateStatistics { get; set; }

        [XmlElement("PV")]
        public int PV { get; set; }

        [XmlElement("UV")]
        public int UV { get; set; }
        [XmlElement("VIP")]
        public int VIP { get; set; }

        [XmlElement("GoldenVIP")]
        public int GoldenVIP { get; set; }

        [XmlElement("PlatinaVIP")]
        public int PlatinaVIP { get; set; }

        [XmlElement("DiamondVIP")]
        public int DiamondVIP { get; set; }

        [XmlElement("OrderNums")]
        public double OrderNums { get; set; }

        [XmlElement("ListingOutletFlag")]
        public short ListingOutletFlag { get; set; }
    }
}
