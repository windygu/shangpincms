using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.GiftCard
{
    public class GiftCardListInfo
    {
        //卡片类型 	卡号 	面值 	卡内余额 	本次使用 	卡片状态 	有效期
        public int CardType { get; set; }
        public string CardTypeName {
            get
            {
                return GetCardType(CardType);
            }
        }
        public string GiftCardNo { get; set; }
        public decimal Amount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal ThisAmount { get; set; }
        public int Status { get; set; }
        public string StatusName {
            get {
                return GetCardStatus(Status);
            }
        }
        public DateTime DateEnd { get; set; }

        public string GetCardType(int cardType)
        { 
            //1：礼品卡电子卡（电子卡）；
            //2：礼品卡刮刮卡（实物卡）；
            //3：礼品卡磁条卡（实物卡）。
            switch(cardType)
            {
                case 1:
                    return "电子卡";
                case 2:
                    return "实物卡";
                case 3:
                    return "磁条卡";
            }
            return "";
        }
        public string GetCardStatus(int status)
        {
            /*
             1：待入库（实物）；
            2：待启用（实物、电子）；
            3：已启用（实物、电子）；
            4：待发售（电子）；
            5：待激活（电子）；
            6：已激活（实物、电子）；
            7：已使用（实物、电子）；
            8：已冻结（实物、电子）；
            9：已失效（实物、电子）。
             * */
            switch (status)
            {
                case 1:
                    return "待入库";
                case 2:
                    return "待启用";
                case 3:
                    return "已启用";
                case 4:
                    return "待发售";
                case 5:
                    return "待激活";
                case 6:
                    return "已激活";
                case 7:
                    return "已使用";
                case 8:
                    return "已冻结";
                case 9:
                    return "已失效";
            }
            return "";
        }
    }

    

}
