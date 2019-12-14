using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Orders
{
    public class UserTicket
    {
        public string CashTicketNo { get; set; }
        public decimal TicketAmount { get; set; }
        public short IsUsed { get; set; }
        public DateTime DateUseEnd { get; set; }
        public short ConditionType { get; set; }
        public string CategoryNos { get; set; }
        public string BrandNos { get; set; }
        public string ProductNos { get; set; }
        public string SubjectNos { get; set; }
        public decimal OrderAmount { get; set; }
        public string SpecialProductNos { get; set; }
        public short UsedType { get; set; }
        public short IsValid { get; set; }
        public decimal TotalUseAmount { get; set; }
        public string TicketDesc { get; set; }
        public short BrandType { get; set; }
    }
}
