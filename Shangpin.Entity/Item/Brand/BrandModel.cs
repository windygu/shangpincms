using System;
using System.Collections.Generic;
using Shangpin.Entity.Wfs;

namespace Shangpin.Entity.Item.Brand
{
    public class BrandModel
    {
        /// <summary>
        /// 根据BrandNo 获取知名品牌列表
        /// </summary>
        /// <returns></returns>
        public IList<SWfsBrandPics> BrandpicLists { get; set; }

        /// <summary>
        /// 根据BrandNo 热门推荐品牌
        /// </summary>
        /// <returns></returns>
        public IList<SWfsBrandPics> Brandpics { get; set; }

        /// <summary>
        /// 品牌变量名称
        /// </summary>
        /// <returns></returns>
        public String BrandName { get; set; }
        /// <summary>
        /// 品牌英文变量名称
        /// </summary>
        /// <returns></returns>
        public String BrandEnName { get; set; }

        /// <summary>
        /// 品牌馆广告
        /// </summary>
        public IList<SWfsBrandHallAd> BrandHallAds { get; set; }

        /// <summary>
        /// 热门分类
        /// </summary>
        public IList<SWfsBrandHallHot> BrandHallHots { get; set; }

        /// <summary>
        /// 即将入驻的品牌
        /// </summary>
        public IList<WfsBrand> BrandHallVergeofs { get; set; }

        /// <summary>
        /// 品牌排行榜 - 品类
        /// </summary>
        public List<BrandTopAttrModel> BrandHallTopByCategorys { get; set; }

        /// <summary>
        /// 国际知名品牌
        /// </summary>
        public IList<BrandAttrModel> BrandFamousBrands { get; set; }

        /// <summary>
        /// 特色品牌推荐
        /// </summary>
        public IList<SWfsBrandHallSpecialBrand> BrandHallSpecials { get; set; }
        
        /// <summary>
        /// 热门分类特色品牌推荐
        /// </summary>
        public IList<SWfsBrandHallSpecialBrand> BrandLeftSpecials { get; set; }
        public IList<SWfsBrandHallSpecialBrand> BrandRightSpecials { get; set; }
        
        /// <summary>
        /// 热门分类品牌及Logo
        /// </summary>
        public IList<BrandAttrModel> BrandList { get; set; }
                
        /// <summary>
        /// 性别
        /// </summary>
        /// <returns></returns>
        public int BrandGender { get; set; }

    }

    /// <summary>
    /// 品牌排行榜
    /// </summary>
    public class BrandTopAttrModel
    {
        public BrandTopAttrModel()
        {
            BrandTop = new List<BrandAttrModel>();
        }

        /// <summary>
        /// 品牌排行榜表ID
        /// </summary>
        public string BrandHallBrandTopId { get; set; }

        /// <summary>
        /// 分类编号
        /// </summary>
        public string CategoryNo { get; set; }

        /// <summary>
        /// 排行榜名称
        /// </summary>
        public string BrandTopName { get; set; }

        /// <summary>
        /// Top5的品牌 -- 品牌排行榜
        /// </summary>
        public List<BrandAttrModel> BrandTop { get; set; }
    }
}
