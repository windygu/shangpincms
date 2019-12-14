namespace Shangpin.Entity.Item
{
    /// <summary>
    /// 商品标签信息
    /// </summary>
    /// Author:wangtao
    /// Date:2012/8/28
    public class ProductLabel
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        /// <value>
        /// The name of the product label.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/28
        public string ProductLabelName { get; set; }
        /// <summary>
        /// 标签编号
        /// </summary>
        /// <value>
        /// The product label no.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/28
        public string ProductLabelNo { get; set; }
        /// <summary>
        /// 标签分类名称
        /// </summary>
        /// <value>
        /// The name of the product label type.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/28
        public string ProductLabelTypeName { get; set; }
        /// <summary>
        /// 标签分类编号
        /// </summary>
        /// <value>
        /// The product label type no.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/28
        public string ProductLabelTypeNo { get; set; }
        /// <summary>
        /// 标签排序
        /// </summary>
        /// <value>
        /// The product label sort.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/28
        public int ProductLabelSort { get; set; }
        /// <summary>
        /// 标签分类排序
        /// </summary>
        /// <value>
        /// The product label type sort.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/28
        public int ProductLabelTypeSort { get; set; }
    }
}
