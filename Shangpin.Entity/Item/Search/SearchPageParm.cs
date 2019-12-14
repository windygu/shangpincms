using System.Collections.Generic;

namespace Shangpin.Entity.Item.Search
{
    /// <summary>
    /// 搜索结果页查询参数
    /// </summary>
    public class SearchPageParm
    {
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// 起始条数
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// 结束条数
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// gender==1男士为 "A02", 女士为"A01"
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 排序类型0没有 1按价格 2按上架时间
        /// </summary>
        public short SortBy{ get; set; }

        /// <summary>
        /// 排序顺序1升序 2降序
        /// </summary>
        public short SortType { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortByField { get; set; }

        /// <summary>
        /// 用户IP
        /// </summary>
        public string UserIP{ get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID{ get; set; }

        /// <summary>
        /// 分类编号
        /// </summary>
        public string CategoryNO { get; set; }

        /// <summary>
        /// 品牌编号
        /// </summary>
        public string BrandNO { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        public string PrimaryColorId{get;set;}

        /// <summary>
        /// 尺码
        /// </summary>
        public string ProductSize{get;set;}
        
        /// <summary>
        /// 尚品价、普通会员价 价格区间
        /// </summary>
        public string LimitedPrice{get;set;}

        /// <summary>
        /// 是否有库存 0：无库存，1：有库存
        /// </summary>
        public string HasStock{get;set;}

        /// <summary>
        /// 价格区间过滤字段---根据当前会员级别显示
        /// </summary>
        public string PriceZone { get; set; }

        public string Price { get; set; }
    }


    /// <summary>
    /// （男女）列表页,品牌列表页产品搜索查询参数
    /// </summary>
    public class SearchListParm
    {

        /// <summary>
        /// 性别
        /// </summary>
        public short Gender { get; set; }

        /// <summary>
        /// 起始条数
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// 结束条数
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// 类别基础品类
        /// </summary>
        public string BaseCategory { get; set; }

        /// <summary>
        /// 过滤类型 1价格 2按上架时间（新品） 3库存
        /// </summary>
        public short SortBy { get; set; }

        /// <summary>
        /// 排序顺序1升序 2降序
        /// </summary>
        public short SortType { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortByField { get; set; }

        /// <summary>
        /// 用户IP
        /// </summary>
        public string UserIP { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 尺寸
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// 查询价格区间索引主键
        /// </summary>
        public string PriceIndex { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// 品类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 价格区间过滤字段---根据当前会员级别显示
        /// </summary>
        public string PriceZone { get; set; }

        /// <summary>
        /// 查询的价格区间字符串1000~2000
        /// </summary>
        public string PriceSelector { get; set; }

        /// <summary>
        /// 显示产品类别的标签 1新品 2售罄 3待定根据前台需求传输不同的值解析成相应的查询字段名称
        /// </summary>
        public string ProductFlag { get; set; }

        /// <summary>
        /// 查询指定价格的产品---价格为固定数20130115新增
        /// </summary>
        public string marketPrice { get; set; }

        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 最新到货时间
        /// </summary>
        public string ArrivalTime { get; set; }

        /// <summary>
        /// 是否只显示有库存商品
        /// </summary>
        public string HasStock { get; set; }

        /// <summary>
        /// 表示当前点击的筛选条件
        /// 1品类，2品牌，3颜色 4尺码  5价格 6趋势 7材质 8最新到货日期
        /// </summary>
        public string Tag { get; set; }

    }

    /// <summary>
    /// 品牌查询参数
    /// </summary>
    public class SearchBrandParm
    {
        /// <summary>
        /// 类别
        /// </summary>
        public string CategoryNo { get; set; }

        /// <summary>
        /// 排序顺序
        /// </summary>
        public string OrderFieldStr { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public short Gender { get; set; }

        /// <summary>
        /// 用户IP
        /// </summary>
        public string UserIP { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

    }

    /// <summary>
    /// 搜索关键词查询参数
    /// </summary>
    public class SearchRelatedKeywordParam
    { 
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 用户IP
        /// </summary>
        public string UserIP { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }
    }


    /// <summary>
    /// 搜索关键词提示
    /// </summary>
    public class AutoCompleteJson
    {
        public string kw { get; set; }
        public IList<Sub> sub { get; set; }
        public string count { get; set; }
    }
    public class Sub
    {
        public string id { get; set; }
        public string cate { get; set; }
        public string count { get; set; }
    }
}
