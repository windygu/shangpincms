using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Trade
{
    public class GiftCardOrderInfo
    {
        //礼品卡订单信息
        public string ProductNo { get; set; }
        //存放属性值
        public string DynamicAttributeText { get; set; }           
        //购买数量
        public int Quantity { get; set; }
        //shangpin:1:categoryNo=0 ,2:categoryNo=专题号
        //outlets:1:categoryNo=活动子类编号，2:categroryNo=专题号
        public short TopicSubjectFlag { get; set; }
        public string CategoryNo { get; set; }
        public string SkuNo { get; set; }
        public string PromotionGroupNo { get; set; }
        public decimal Price { get; set; }

        //1：礼品卡电子卡（电子卡）2：礼品卡刮刮卡（实物卡）3：礼品卡磁条卡（实物卡） 
        public short CardType { get; set; }
        //购买类型：0普通购买(所有普通商品) 1立即购买(礼品卡实物卡)
        public short BuyType { get; set; }

        //赠送人信息
        public string ReciveName { get; set; }
        public string FromName { get; set; }
        public string PresentInfo { get; set; }
        public string ReciveMobileNo { get; set; }
        public decimal EGiftCardAmount { get; set; }
    }
}
