using System;
using System.Collections.Generic;
using Shangpin.Entity.GiftCard;
using Shangpin.Entity.Trade;
using Shangpin.Entity.User;
using Shangpin.Entity.Wfs;
using Shangpin.Entity.GiftCard;

namespace Shangpin.Entity.Payment
{
    public class PaywayModel
    {
        public CustomerInfo Customer { get; set; }
        public List<Group> OrderDetailList { get; set; }
        public WfsFedexCod FedexCodeSingle { get; set; }
        public WfsOrder OrderSingle { get; set; }
        public int ProductCount { get; set; }
        public PaymentTimeInfo PaymentStatusForTime { get; set; }
        public BankActivityInfo BankActivityResult { get; set; }
        public Boolean IsRuFengDaContain { get; set; }
        public Boolean IsiPad { get; set; }
        public List<GiftCardListInfo> GiftCardList { get; set; }
        public Boolean IsUsableGiftCardPay { get; set; }
        public List<GiftCardInfo> AlreadyGiftCardList { get; set; }
        public Boolean IsLockCardAccount { get; set; }
        public Boolean IsElGiftCard { get; set; }
        public Boolean IsSandPay { get; set; }
    }
}