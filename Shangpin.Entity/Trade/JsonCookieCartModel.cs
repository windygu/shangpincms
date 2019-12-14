using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Trade
{
    public class JsonCookieCartModel
    {
        public string ShoppingCartDetailId { get; set; }
        public string ProductNo { get; set; }
        //存放属性值
        public string DynamicAttributeText { get; set; }
        //public Dictionary<string,string> DynamicAttributeDic { get; set; }        
        //购买数量
        public int Quantity { get; set; }

        //shangpin:1:categoryNo=0 ,2:categoryNo=专题号
        //outlets:1:categoryNo=活动子类编号，2:categroryNo=专题号
        public short TopicSubjectFlag { get; set; }
        public string CategoryNo { get; set; }
        public string VipNo { get; set; }
        public string SkuNo { get; set; }
        public DateTime DateAdd { get; set; }
        public string TopicNo { get; set; }
        public string GroupId { get; set; }
        public string GroupNo { get; set; }
        public decimal TotalGroupPrice { get; set; }

    }
}
