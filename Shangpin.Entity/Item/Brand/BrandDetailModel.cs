using System.Collections.Generic;
using Shangpin.Entity.Item.Search;
using Shangpin.Entity.User;
using Shangpin.Entity.Wfs;

namespace Shangpin.Entity.Item.Brand
{
    public class BrandDetailModel
    {
        /// <summary>
        /// 品牌详情实体类
        /// </summary>
        /// <returns></returns>
        public WfsBrand BrandDetailContent { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        /// <returns></returns>
        public CustomerInfo Customer{ get; set; }

        /// <summary>
        /// 搜索产品信息
        /// </summary>
        /// <returns></returns>
        public SearchResultInfo SearchResult { get; set; }
        
        /// <summary>
        /// 分类目录
        /// </summary>
        /// <returns></returns>
        public IList<SWfsCategory> CategoryList { get; set; }
        
        /// <summary>
        /// 从搜索中查询分类
        /// </summary>
        /// <returns></returns>
        public IList<SWfsSpecialRecommendation> SpecialRecommend { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        /// <returns></returns>
        public int BrandGender { get; set; }         
    }
}