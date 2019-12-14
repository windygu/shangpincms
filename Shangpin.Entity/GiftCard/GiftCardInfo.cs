using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.GiftCard
{
    public class GiftCardInfo
    {
        public string GiftCardNo {get;set;}
        public decimal CurrentAmount {get;set;}
        public decimal PayAmount {get;set;}
        public DateTime DateEnd{get;set;}
        public short Status { get; set; }
        public short CardType { get; set; }
        public string CardDateEnd { get; set; }
        public decimal Amount { get; set; }
        public string CardAmount { get; set; }
    }
}
