namespace Shangpin.Entity.Customers
{
    /// <summary>
    /// 购买过的商品
    /// </summary>
    public class BuyedProductInfo
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductNo { get; set; }
        /// <summary>
        /// 商品图片
        /// </summary>
        public string ProductPic{get ;set ;}
        /// <summary>
        /// 品牌英文名称
        /// </summary>
        public string BrandEnName { get; set; }
        /// <summary>
        /// 品牌中文名称
        /// </summary>
        public string BrandCnName { get; set; }
    }
}
