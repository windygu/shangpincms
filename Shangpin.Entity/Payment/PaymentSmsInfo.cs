namespace Shangpin.Entity.Payment
{
    public class PaymentSmsInfo
    {
        public string ConsigneeMobile { get; set; }
        public decimal PayAmount { get; set; } 
        public string TrueName { get; set; }
        public short OrderStyle { get; set; }
        public string OrderUserId { get; set; }  
    } 
}
