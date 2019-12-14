using System.ComponentModel;

namespace Shangpin.Entity.Item.Search
{
    public enum SearchUrlFromEnum
    {
        [Description("URL来源为列表页")]
        Listing=1,
        [Description("URL来源为品牌列表页")]
        BrandListing=2,
        [Description("URL来源为新品列表页")]
        NewListing=3,
        [Description("URL来源为搜索框")]
        Search=4,
        [Description("URL来源为品牌新品列表页")]
        BrandNewListing=5
    }
    public class PriceSetting
    {
        /// <summary>
        /// 显示标题
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/29
        public string Text { get; set; }
        /// <summary>
        /// 索引
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/29
        public int Index { get; set; }
        /// <summary>
        /// 最大金额
        /// </summary>
        /// <value>
        /// The max value.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/29
        public int MaxValue { get; set; }
        /// <summary>
        /// 最小金额
        /// </summary>
        /// <value>
        /// The min value.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/29
        public int MinValue { get; set; }

        /// <summary>
        /// 价格区间值
        /// </summary>
        public string PriceValue { get; set; }
    }
}
