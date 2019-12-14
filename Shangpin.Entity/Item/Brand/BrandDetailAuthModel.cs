using System;
using System.Collections.Generic;
using Shangpin.Entity.Wfs;

namespace Shangpin.Entity.Item.Brand
{
    public class BrandDetailAuthModel
    {
        /// <summary>
        /// 根据BrandNo 获取新品牌展示页面   品牌列表左侧新品到货索引
        /// </summary>
        /// <returns></returns>
        public IList<SWfsBrandNewArrival> BrandNewList { get; set; } 
        /// <summary>
        /// 根据BrandNo 获取新品牌 图片
        /// </summary>
        /// <returns></returns>
        public BrandDetailAuthInfo BrandDetailContent { get; set; }

        /// <summary>
        /// 新品牌信息显示异性分类
        /// </summary>
        /// <returns></returns>
        public BrandDetailAuthInfo BrandDetailGender { get; set; }
        
        /// <summary>
        /// 根据BrandNo 获取Category二级分类
        /// </summary>
        /// <returns></returns>
        public IList<CategoryNoInfo> BrandCategory { get; set; }
        
        /// <summary>
        /// 根据BrandNo 获取新品牌图片 iframe 的信息
        /// </summary>
        /// <returns></returns>
        public IList<SWfsBrandIframes> BrandIframe { get; set; }

        /// <summary>
        /// 根据BrandNo 获取品牌图片 
        /// </summary>
        /// <returns></returns>
        public IList<SWfsBrandPics> BrandBanner { get; set; }
        /// <summary>
        /// 根据BrandNo 获取品牌图片 
        /// </summary>
        /// <returns></returns>
        public IList<SWfsBrandPics> BrandArchive { get; set; }

        /// <summary>
        /// BrandNo 
        /// </summary>
        /// <returns></returns>
        public String BrandNo { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        /// <returns></returns>
        public int BrandGender { get; set; }         
    }
}