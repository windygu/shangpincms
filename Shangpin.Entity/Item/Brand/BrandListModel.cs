using System;
using System.Collections.Generic;
using Shangpin.Entity.Item.Search;
using Shangpin.Entity.User;
using Shangpin.Entity.Wfs;

namespace Shangpin.Entity.Item.Brand
{
    public class BrandListModel
    {    
        /// <summary>
        /// 根据BrandNo 获取品牌展示页面   品牌列表左侧新品到货索引
        /// </summary>
        /// <returns></returns>
        public IList<SWfsCategory> CategoryList { get; set; }
        /// <summary>
        /// 根据BrandNo 获取品牌
        /// </summary>
        /// <returns></returns>
        public WfsBrand BrandDetailContent { get; set; }
         /// <summary>
        ///品牌特别推荐
        /// </summary>
        /// <returns></returns>
        public IList<SWfsSpecialRecommendation> SpecialRecommend { get; set; }
        
        /// <summary>
        /// 用户信息
        /// </summary>
        /// <returns></returns>
        public CustomerInfo Customer { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        /// <returns></returns>
        public int BrandGender { get; set; }
        /// <summary>
        /// 品牌BrandNo
        /// </summary>
        /// <returns></returns>
        public String BrandNo { get; set; }

        /// <summary>
        /// 显示产品列表
        /// </summary>
        public ProductListParams ListParam { get; set; }

    }
}