namespace Shangpin.Entity.Item
{
    /// <summary>
    /// 商品终端页展示属性
    /// </summary>
    /// 
    /// 
    public class ProductProperty
    {
        /// <summary>
        /// 商品属性类型
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        /// 
        /// 
        public int PropertyType { get; set; }
        /// <summary>
        /// 商品属性值
        /// </summary>
        /// <value>
        /// The property value.
        /// </value>
        /// 
        /// 
        public int PropertyValue { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        /// <value>
        /// The name of the property attr.
        /// </value>
        /// 
        /// 
        public string PropertyAttrName { get; set; }
        /// <summary>
        /// 属性标题
        /// </summary>
        /// <value>
        /// The property attr title.
        /// </value>
        /// 
        /// 
        public string PropertyAttrTitle { get; set; }
        /// <summary>
        /// 属性内容值
        /// </summary>
        /// <value>
        /// The property attr value.
        /// </value>
        /// 
        /// 
        public string PropertyAttrValue { get; set; }
    }
}
