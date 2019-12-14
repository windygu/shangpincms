using System.ComponentModel;

namespace Shangpin.Entity.Payment
{
    public struct PaymentBrankType
    {
        [Description("工商银行")]
        public const int BankICBC = 32;
        [Description("农行银行")]
         public const int   BankABC = 33;
        [Description("交通银行")]
         public const int   BankBankComm = 1;
        [Description("中国银行")]
         public const int   BankBocChina = 3;
        [Description("建行银行")]
         public const int   BankCCB = 6;
        [Description("光大银行")]
         public const int   BankCEB = 29;
        [Description("民生银行")]
        public const int    BankCMBCGatePay = 15;
        [Description("招行银行")]
         public const int   BankCmbChina = 2;
        [Description("广发银行")]
         public const int   BankGDB = 30;
        [Description("浦发银行")]
        public const int    BankSpdbBank = 4;
        [Description("银联")]
        public const int BankUnionPay = 27;
        [Description("支付宝支付")]
         public const int   BankAliPayment= 5;
        [Description("支付宝信用卡支付")]
        public const int BankAlipayMoto = 13;
        [Description("拉卡拉支付")]
         public const int   BankLakala = 24;
        [Description("快钱支付")]
         public const int   BankNBillPayment = 10;
        [Description("平安银行")]
        public const int BankPingAn = 42;
        [Description("交通银行银行")]
        public const int BOCMCCreditCardPay = 44;
        [Description("杉德支付")]
        public const int sandPay = 45;
        [Description("上海导购")]
        public const int BankSHDG = 47;
    }
}
