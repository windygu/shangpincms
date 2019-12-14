using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    [Serializable]
    public class WfsBankFQPayM
    {
        public decimal AmountReturn { get; set; }
        public decimal AmountSend { get; set; }
        public string BankFQInfoNo { get; set; }
        public string BankFQOrderNo { get; set; }
        public int Currency { get; set; }
        public string ExcepitonMsg { get; set; }
        public string MerchantNo { get; set; }
        public DateTime PayDate { get; set; }
        public short PayResult { get; set; }
        public short Period { get; set; }
        public string PostData { get; set; }
        public string RetureData { get; set; }
        public string ShangOrderNo { get; set; }
        public string Signature { get; set; }
        public DateTime TradeDate { get; set; }

        /// <summary>
        /// 招行内部订单号
        /// </summary>
        public string CMBOrderNo { get; set; }
    }
}
