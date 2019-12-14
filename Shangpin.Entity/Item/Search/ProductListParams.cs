using System.Collections.Generic;
using Shangpin.Entity.User;
using Shangpin.Entity.Wfs;

namespace Shangpin.Entity.Item.Search
{
    /// <summary>
    /// 商品列表页参数
    /// </summary>
    /// Author:wangtao
    /// Date:2012/7/23
    public class ProductListParams
    {
        /// <summary>
        /// 当前用户信息
        /// </summary>
        /// <value>
        /// The customer info.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/23
        public CustomerInfo CustomerInfo { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        /// <value>
        /// The total page count.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/23
        public int TotalPageCount { get; set; }
        /// <summary>
        /// Gets or sets the SearchResultInfo.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/23
        public SearchResultInfo SearchResultInfo { get; set; }

        /// <summary>
        /// 分类列表
        /// </summary>
        /// <value>
        /// The category list.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/23
        public IList<CategoryInfo> CategoryList { get; set; }
        /// <summary>
        /// 品牌列表
        /// </summary>
        /// <value>
        /// The brand list.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/23
        public IList<BrandInfo> BrandList { get; set; }

        /// <summary>
        /// 品牌字母索引
        /// </summary>
        /// <value>
        /// The index of the brand.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/28
        public IList<string> BrandIndex { get; set; }

        /// <summary>
        /// 商品尺码列表
        /// </summary>
        /// <value>
        /// The product size list.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/6
        public IList<string> ProductSizeList { get; set; }
        /// <summary>
        /// 特别推荐
        /// </summary>
        /// <value>
        /// The index of the special recommend.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/6
        public IList<SWfsSpecialRecommendation> SpecialRecommendIndex { get; set; }
        ///// <summary>
        ///// 是否有下次的AJAx加载区块
        ///// </summary>
        ///// <value>
        ///// 	<c>true</c> if this instance has next block; otherwise, <c>false</c>.
        ///// </value>
        ///// Author:wangtao
        ///// Date:2012/8/24
       public bool HasNextBlock { get; set; }

        /// <summary>
        /// 列表页查询参数
        /// </summary>
        /// <value>
        /// The query params.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/24
        public ProductListQueryParams QueryParams { get; set; }
        /// <summary>
        /// 标签列表
        /// </summary>
        /// <value>
        /// The label list.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/28
        public Dictionary<string, IList<ProductLabel>> LabelList { get; set; }
        /// <summary>
        /// 颜色列表
        /// </summary>
        /// <value>
        /// The color list.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/28
        public Dictionary<string, KeyValuePair<string, string>> ColorList { get; set; }

        /// <summary>
        /// 20130329版本颜色列表--BYLIULEI
        /// </summary>
        public IList<ListColor> NewColorList { get; set; }

        /// <summary>
        /// 价格筛选列表
        /// </summary>
        /// <value>
        /// The price list.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/29
        public IList<PriceSetting> PriceList { get; set; }

        /// <summary>
        /// 头图
        /// </summary>
        /// <value>
        /// The head banner pic file no.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/28
        public SWfsCategoryLabelPicture HeadBannerPicFileNo { get; set; }

        /// <summary>
        /// 鞋靴类广告图
        /// </summary>
        public IList<SWfsCategoryAdPicture> CategoryAdPictureList { get; set; }
        /// <summary>
        /// 鞋靴类推荐新品
        /// </summary>
        public IList<ProductInfo> TJNewProductList { get; set; }
        public IList<SWfsSpecialRecommendation> SpecialRecommendationText { get; set; }

        /// <summary>
        /// 相关搜索
        /// </summary>
        public IList<string> RelatedKeywordList { get; set; }

        /// <summary>
        /// 最新到货时间
        /// </summary>
        public IList<NewArrivalInfo> ArrivalTimeList { get; set; }
    }
}
