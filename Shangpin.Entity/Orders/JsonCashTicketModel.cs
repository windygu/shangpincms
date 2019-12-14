using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Orders
{
    public class JsonCashTicketModel
    {
        public string cashticketNo { get; set; }
        public string ticketAmount { get; set; }
        public string totalUseAmount { get; set; }
        public string dateUseEnd { get; set; }
        public string ticketDesc { get; set; }
    }
}
