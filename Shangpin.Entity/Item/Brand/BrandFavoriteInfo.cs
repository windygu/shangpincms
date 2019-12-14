namespace Shangpin.Entity.Item.Brand
{
    /// <summary>
    /// 收藏列表统计信息
    /// </summary>
    public class BrandFavoriteInfo
    {  
       /// <summary>
       ///收藏数量
       /// </summary>
       public int? FavoriteCount { get; set; }

       /// <summary>
       ///是否收藏过
       /// </summary>
       public int? IsFavorite { get; set; }

       /// <summary>
       ///是否已经登录，已登录读取收藏该品牌的ID号
       /// </summary>
       public int? FavoriteId{ get; set; } 
    }
}
