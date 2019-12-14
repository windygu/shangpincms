using System.Collections.Generic;
using Shangpin.Entity.Item.Search;
using Shangpin.Entity.Wfs;

namespace Shangpin.Entity.Item.Brand
{
    /// <summary>
    /// 品牌终端页
    /// </summary>
    public class BrandsDetailModel
    {
        public WfsBrand CurrentBrand { get; set; }
        /// <summary>
        /// 品牌编号
        /// </summary>
        public string BrandNo { get; set; }
        /// <summary>
        /// 页面背景图
        /// </summary>
        public string BackgroundPic
        {
            get;
            set;
        }
        /// <summary>
        /// 页面背景颜色
        /// </summary>
        public string BackgroundColor
        { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }
        /// <summary>
        /// 是否有新品
        /// </summary>
        public bool ProductNewFlag { get; set; }
        /// <summary>
        /// 品类
        /// </summary>
        public IList<CategoryNoInfo> BrandCategory { get; set; }
        /// <summary>
        /// 广告位(右上)
        /// </summary>
        public IList<SWfsBrandPics> BrandAd { get; set; }
        /// <summary>
        /// 显示产品列表
        /// </summary>
        public ProductListParams ListParam { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int BrandGender { get; set; }
        /// <summary>
        /// 顶通图
        /// </summary>
        public string BrandTopPic { get; set; }
        /// <summary>
        /// 是否关注
        /// </summary>
        public bool IsConcern { get; set; }
        /// <summary>
        /// 关注数量
        /// </summary>
        public int ConcernCount { get; set; }
        /// <summary>
        /// 去除http;//www.shangpin.com的Url
        /// </summary>
        public string BrandUrl { get; set; }
        public bool IsClickNew { get; set; }
    }
}