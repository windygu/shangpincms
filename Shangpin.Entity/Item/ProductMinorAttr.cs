namespace Shangpin.Entity.Item
{
    public class ProductMinorAttr
    {
        /// <summary>
        /// 商品属性名称(商品尺码?)
        /// </summary>
        /// <value>
        /// The name of the product attr.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/12
        public string ProductAttrName { get; set; }
        /// <summary>
        /// 第二个商品属性值(尺码?)
        /// </summary>
        /// <value>
        /// The product attr value.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/12
        public string ProductAttrValue { get; set; }
        /// <summary>
        /// 国际码
        /// </summary>
        /// <value>
        /// The size of the internatinal.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/13
        public string InternatinalSize { get; set; }
        /// <summary>
        /// 标准码名称
        /// </summary>
        /// <value>
        /// The name of the standar.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/13
        public string StandarName { get; set; }
        /// <summary>
        /// Gets or sets the product attr hash value.
        /// </summary>
        /// <value>
        /// The product attr hash value.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/12
        public string ProductAttrHashValue { get; set; }
        /// <summary>
        /// 排序值
        /// </summary>
        /// <value>
        /// The size order.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/13
        public decimal SizeOrder { get; set; }

        public string CategoryNo { get; set; }

        public short SizeStandard { get; set; }

        public string Size { get; set; }
    }
}
