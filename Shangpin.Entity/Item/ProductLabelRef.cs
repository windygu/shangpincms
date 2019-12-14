namespace Shangpin.Entity.Item
{
    /// <summary>
    /// 商品终端页 , 商品指数里的 标签信息使用
    /// </summary>
    /// author:zhaoxiaojun
    /// date:2012.10.08
    public class ProductLabelRef
    {
        /// <summary>
        /// 商品标签名称
        /// </summary>
        public string ProductLabelRefName{get;set;}

        /// <summary>
        /// 商品标签对应的商品编号
        /// </summary>
        public string ProductNo { get; set; }

        /// <summary>
        /// 商品标签编号
        /// </summary>
        public string ProductLabelNo{get;set;}

        /// <summary>
        /// 商品标签对应的标签详情编号
        /// </summary>
        public string ProductLabelDetailNo{get;set;}

        /// <summary>
        /// 标签类型
        /// </summary>
        public short LabelType{get;set;}

        /// <summary>
        /// 商品指数 , 标签选中内容
        /// </summary>
        public string SelectLabelContent { get; set; }

        /// <summary>
        /// 商品指数排序
        /// </summary>
        public int SortValue { get; set; }

        /// <summary>
        /// 商品标签图片
        /// </summary>
        public string LabelDetailPicNo { get; set; }
    }
}
