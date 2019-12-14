using System.Runtime.Serialization;

namespace Shangpin.Entity.Item
{
    /// <summary>
    /// 产品库存信息
    /// </summary>
    
    public class ProductInventoryInfo
    {

        public string SkuNo { get; set; }

        public string ProductNo { get; set; }

        public string Color { get; set; }

        public string  Size {get;set;}

        public string  NewSize {get;set;}

        public string  SizeOrder {get;set;}
  
        public int  Quantity {get;set;}

        public int  RealQuantity {get;set;}

        public decimal  SellPrice {get;set;}

        public string SkuHashText { get; set; }

        /// <summary>
        /// SKU是否有效
        /// </summary>
        /// <value>
        /// The SKU is valid.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/22
        [DataMember]
        public int SkuIsValid { get; set; }

        public int SkuType { get; set; }

    }
}
