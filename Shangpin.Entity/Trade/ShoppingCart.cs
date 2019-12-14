using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Trade
{
    public class ShoppingCart
    {
        public string ShoppingCartId { get; set; }
        public string Session { get; set; }
        public int SkuCount { get; set; }
        public string ItemXmlText { get; set; }
        public int DeliverId { get; set; }
        public short PayTypeId { get; set; }
        public int PayTypeChildId { get; set; }
        public string DeliverName { get; set; }
        public decimal Freight { get; set; }
        public short InvoiceFlag { get; set; }
        public string InvoiceTitle { get; set; }
        public int TimeRequest { get; set; }
        public string SpecialRequest { get; set; }
        public short InvoiceType { get; set; }      
    }
}
