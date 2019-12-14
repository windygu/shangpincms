using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.GiftCard
{
    public class GiftCardCostJsonModel
    {
        public string CardType { get; set; }
        public string GiftCardNo { get; set; }
        public string Amount { get; set; }
        public string CurrentAmount { get; set; }
        public string Status { get; set; }
        public string DateEnd { get; set; }

        public List<GiftCardCostDetail> list{get;set;}
    }

    public class GiftCardCostDetail
    {
        public string OrderNum { get; set; }
        public string OrderDate { get; set; }
        public DateTime DateCreate { get; set; }
        public string UseType { get; set; }
        public decimal BeforeVal { get; set; }
        public decimal ChangeVal { get; set; }
        public decimal AfterVal { get; set; }
        public short ListOutletFlag { get; set; }
        public string LinkUrl { get; set; }
    }
}
