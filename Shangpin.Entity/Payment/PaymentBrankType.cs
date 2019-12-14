using System.ComponentModel;

namespace Shangpin.Entity.Payment
{
    public struct PaymentBrankType
    {
        [Description("��������")]
        public const int BankICBC = 32;
        [Description("ũ������")]
         public const int   BankABC = 33;
        [Description("��ͨ����")]
         public const int   BankBankComm = 1;
        [Description("�й�����")]
         public const int   BankBocChina = 3;
        [Description("��������")]
         public const int   BankCCB = 6;
        [Description("�������")]
         public const int   BankCEB = 29;
        [Description("��������")]
        public const int    BankCMBCGatePay = 15;
        [Description("��������")]
         public const int   BankCmbChina = 2;
        [Description("�㷢����")]
         public const int   BankGDB = 30;
        [Description("�ַ�����")]
        public const int    BankSpdbBank = 4;
        [Description("����")]
        public const int BankUnionPay = 27;
        [Description("֧����֧��")]
         public const int   BankAliPayment= 5;
        [Description("֧�������ÿ�֧��")]
        public const int BankAlipayMoto = 13;
        [Description("������֧��")]
         public const int   BankLakala = 24;
        [Description("��Ǯ֧��")]
         public const int   BankNBillPayment = 10;
        [Description("ƽ������")]
        public const int BankPingAn = 42;
        [Description("��ͨ��������")]
        public const int BOCMCCreditCardPay = 44;
        [Description("ɼ��֧��")]
        public const int sandPay = 45;
        [Description("�Ϻ�����")]
        public const int BankSHDG = 47;
    }
}
