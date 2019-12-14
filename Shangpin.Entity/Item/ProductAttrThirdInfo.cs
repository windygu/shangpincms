namespace Shangpin.Entity.Item
{
    /// <summary>
    /// 商品属性下第三信息(例如:服装尺码,手表防水性等)
    /// </summary>
    public class ProductAttrThirdInfo
    {
        /// <summary>
        /// 原信息(例如原尺码)
        /// </summary>
        public string OriginalInfo { get; set; }
        /// <summary>
        /// 原信息标准
        /// </summary>
        public string OriginalStandard { get; set; }
        private string _ChangedInfo;
        /// <summary>
        /// 转换后的信息(国际码或中国码)
        /// </summary>
        public string ChangedInfo
        {
            get
            {
                if (string.IsNullOrEmpty(_ChangedInfo))
                {
                    _ChangedInfo = OriginalInfo;
                }
                return _ChangedInfo;
            }
            set
            {
                _ChangedInfo = value;
            }

        }
        /// <summary>
        /// 排序索引
        /// </summary>
        public decimal SizeOrder { get; set; }
        /// <summary>
        /// 商品哈希文本
        /// </summary>
        public string SkuHashText { get; set; }
    }
}
