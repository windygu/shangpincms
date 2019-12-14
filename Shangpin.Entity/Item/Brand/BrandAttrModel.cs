namespace Shangpin.Entity.Item.Brand
{
    /// <summary>
    /// 品牌馆属性对象
    /// </summary>
    /// Author:wwp
    /// Date:2012-10-16
    public class BrandAttrModel
    {
        /// <summary>
        /// 分类编号
        /// </summary>
        public string CategoryNo { get; set; }

        /// <summary>
        /// 排行榜名称
        /// </summary>
        public string BrandTopName { get; set; }

        /// <summary>
        /// 排行榜类型（1:国际知名品牌）
        /// </summary>
        public short TopType { get; set; }

        /// <summary>
        /// 品牌馆排行榜表id
        /// </summary>
        public string BrandHallBrandTopId { get; set; }

        /// <summary>
        /// 品牌编号
        /// </summary>
        public string BrandNo { get; set; }

        /// <summary>
        /// 品牌中文名
        /// </summary>
        public string BrandCnName { get; set; }

        /// <summary>
        /// 品牌英文名
        /// </summary>
        public string BrandEnName { get; set; }

        /// <summary>
        /// 品牌图片
        /// </summary>
        public string BrandPicNo { get; set; }
    }
}
