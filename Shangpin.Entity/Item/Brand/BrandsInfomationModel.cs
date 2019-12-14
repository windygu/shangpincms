using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Entity.User;

namespace Shangpin.Entity.Item.Brand
{
    /// <summary>
    /// 品牌资讯
    /// </summary>
    public class BrandsInfomationModel
    {
        public string BrandNo { get; set; }
        /// <summary>
        /// 页面背景图
        /// </summary>
        public string BackgroundPic { get; set; }
        /// <summary>
        /// 页面背景色
        /// </summary>
        public string BackgroundColor { get; set; }
        /// <summary>
        /// 品牌顶通图
        /// </summary>
        public string BrandTopPic { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }
        /// <summary>
        /// 是否新品到货
        /// </summary>
        public bool ProductNewFlag { get; set; }
        /// <summary>
        /// 品牌介绍
        /// </summary>
        public string BrandInfo { get; set; }
        /// <summary>
        /// 视频资讯
        /// </summary>
        public IList<SWfsBrandPics> VideoInfomation { get; set; }
        /// <summary>
        /// 海报图大图
        /// </summary>
        public IList<SWfsBrandPics> PosterPicBig { get; set; }
        /// <summary>
        /// 海报图小图
        /// </summary>
        public IList<SWfsBrandPics> PosterPicSmall { get; set; }
        /// <summary>
        /// 单品介绍
        /// </summary>
        public IList<SWfsBrandPics> SingleInfo { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int BrandGender { get; set; }
        /// <summary>
        /// 是否关注
        /// </summary>
        public bool IsConcern { get; set; }
        /// <summary>
        /// 关注人数
        /// </summary>
        public int ConcernCount { get; set; }
        /// <summary>
        /// 品类
        /// </summary>
        public IList<CategoryNoInfo> BrandCategory { get; set; }
        /// <summary>
        /// 当前品牌
        /// </summary>
        public WfsBrand CurrentBrand { get; set; }
        /// <summary>
        /// 当前账户
        /// </summary>
        public CustomerInfo Customer { get; set; }
        /// <summary>
        /// 去除http;//www.shangpin.com的URL
        /// </summary>
        public string BrandUrl { get; set; }

    }
}
