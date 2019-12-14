using System;

namespace Shangpin.Entity.Item.Brand
{
    public class BrandFavoritesModel
    {
        /// <summary>
        ///  用户ID
        /// </summary>
        /// <returns></returns>
        public String AuthenticationUserId { get; set; }

        /// <summary>
        /// 收藏ID
        /// </summary>
        /// <returns></returns>
        public int? FavoriteBrandId { get; set; }

        /// <summary>
        /// 根据BrandNo 获取品牌中（关注新品牌，收藏品牌）信息 统计数量
        /// </summary>
        /// <returns></returns>
        public BrandFavoriteInfo BrandFavCount { get; set; }
        
        /// <summary>
        /// 品牌BrandNo
        /// </summary>
        /// <returns></returns>
        public String BrandNo { get; set; }

        /// <summary>
        /// 是否显示关注人数
        /// </summary>
        /// <returns></returns>
        public int IsFav { get; set; }
        /// <summary>
        /// brandUrl返回url
        /// </summary>
        public string BrandUrl { get; set; }
        
    }
}