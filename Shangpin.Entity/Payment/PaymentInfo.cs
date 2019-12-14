namespace Shangpin.Entity.Payment
{
    public class PaymentInfo
    {
        /// <summary>
        /// ������
        /// </summary>
        public string OrderNo { get; set; } 
        /// <summary>
        /// ֧��ID��//֧����ʹ��
        /// </summary>
        public string PayTypeChildId{ get; set; }
        /// <summary>
        /// ֧�����ͣ�15����ֱ�� 
        /// </summary>
        public string PayTypeId{ get; set; }
        /// <summary>
        /// ֧��ID��//֧����ʹ�� bankid
        /// </summary>
        public string BankId{ get; set; }   
        /// <summary>
        /// ֧��ID��//֧����ʹ�� directPay ,bankPay,motoPay
        /// </summary>
        public string PayMethod { get; set; }      
        
        /// <summary>
        /// ҳ��֧���ύʱʹ�õ�����
        /// </summary>
        public string  BankName { get; set; } 
    }
}
