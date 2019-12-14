namespace Shangpin.Entity.Trade
{
    public class CartProductInfo
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
        public string CategoryNo { get; set; }//数据统计时用
        public string SkuNo { get; set; }
        //1:普通购买2:修改sku
        public short changeType { get; set; }
        public string GId { get; set; }
        //计算价格时用
        public string VipNo { get; set; }
         
        public string SecondProductNo { get; set; }
        public string SecondDynamicAttributeText { get; set; }
        public string GroupNo { get; set; }
        public string GroupId { get; set; }

        //实物礼品卡
        //1：礼品卡电子卡（电子卡）2：礼品卡刮刮卡（实物卡）3：礼品卡磁条卡（实物卡） 
        public short CardType { get; set; }
        //购买类型：0普通购买(所有普通商品) 1立即购买(礼品卡实物卡)
        public short BuyType { get; set; }
        public decimal Price { get; set; }
    }
}
