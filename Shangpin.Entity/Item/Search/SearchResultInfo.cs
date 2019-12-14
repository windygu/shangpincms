using System.Collections.Generic;

namespace Shangpin.Entity.Item.Search
{

    /// <summary>
    /// 搜索响应头部
    /// </summary>
    /// Author:wangtao
    /// Date:2012/7/10
    public class ResponseHeader
    {
        public int Status { get; set; }
        public decimal QTime { get; set; }
        public int Start { get; set; }
        public int NumFound { get; set; }
    }

    /// <summary>
    /// 搜索结果
    /// </summary>
    /// Author:wangtao
    /// Date:2012/7/10
    public class SearchResultInfo
    {
        /// <summary>
        /// 搜索响应信息
        /// </summary>
        /// <value>
        /// The search response info.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/10
        public ResponseHeader ResponseHeaderInfo { get; set; }
        /// <summary>
        /// 搜索返回商品列表
        /// </summary>
        /// <value>
        /// The product list.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/10
        public IList<ProductInfo> ProductListInfo { get; set; }

        public IList<string> RalatedKeyWordInfo { get; set; }
    }


    public class BrandInfo
    {
        public string BrandNo { get; set; }
        public string BrandEnName { get; set; }
        public string BrandCnName { get; set; }
        public string Display { get; set; }

    }

    public class CategoryInfo
    {
        public string CategoryNo { get; set; }
        public string CategoryName { get; set; }
        public string ParentNo { get; set; }
        public short VisibleState { get; set; }
        public int Sort { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// 最新到货时间
    /// </summary>
    public class NewArrivalInfo
    {
        public string Value { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
    }

    
    /// <summary>
    /// 列表页颜色值
    /// </summary>
    public class ListColor
    {
        /// <summary>
        /// 颜色图片
        /// </summary>
        public string ColorPic { get; set; }

        /// <summary>
        /// 颜色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 颜色主键值
        /// </summary>
        public string KeyValue { get; set; }
    }
}
