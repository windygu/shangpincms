using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Shangpin.Ocs.Entity.Extenstion.ProductFlat
{
    /// <summary>
    /// 前端传值所用产品信息
    /// </summary>
    [Serializable]
    public class ProductInfo
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        [XmlElement]
        public string ProductNo
        {
            get;
            set;
        }

        /// <summary>
        /// OCS分类编号
        /// </summary>
        [XmlElement]
        public string OcsCategoryNo
        {
            get;
            set;
        }

        /// <summary>
        /// 规则ID
        /// </summary>
        [XmlElement]
        public string RuleId
        {
            get;
            set;
        }

        /// <summary>
        /// 排序值
        /// </summary>
        [XmlElement]
        public string SortId
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 前端传值所用规则信息
    /// </summary>
    [Serializable]
    public class Ruels
    {
        public string RuleNo
        {
            get;
            set;
        }
        public string RuleName
        {
            get;
            set;
        }

        public string Sort
        {
            get;
            set;
        }

        public string ParentId
        {
            get;
            set;
        }

        public string RuleType
        {
            get;
            set;
        }

        /// <summary>
        /// 子规则列表
        /// </summary>
        public List<Ruels> RuleList
        {
            get;
            set;
        }

        /// <summary>
        /// 产品列表
        /// </summary>
        public List<ProductInfo> ProductList
        {
            get;
            set;
        }


    }

    /// <summary>
    /// 保存时候页面传值用
    /// </summary>
    [Serializable]
    public class PageDateInfo
    {
        /// <summary>
        /// 分类编号
        /// </summary>
        [XmlElement]
        public string CategoryNo
        {
            get;
            set;
        }

        /// <summary>
        /// 是否存在规则
        /// </summary>
        [XmlElement]
        public string IsRule
        {
            get;
            set;
        }

        /// <summary>
        /// 规则列表
        /// </summary>
        [XmlElement]
        public List<Ruels> RuleList
        {
            get;
            set;
        }

        /// <summary>
        /// 产品列表
        /// </summary>
        public List<ProductInfo> ProductList
        {
            get;
            set;
        }

        /// <summary>
        /// 售罄商品是否自动沉底
        /// </summary>
        public string IsLast
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 产品预览
    /// </summary>
    [Serializable]
    public class ProductView
    {
        /// <summary>
        /// 品牌名称
        /// </summary>
        [XmlElement]
        public string BrandName
        {
            get;
            set;
        }

        /// <summary>
        /// 产品名称
        /// </summary>
        [XmlElement]
        public string ProductName
        {
            get;
            set;
        }

        /// <summary>
        /// 市场价
        /// </summary>
        [XmlElement]
        public string ProductMarketPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 折后价
        /// </summary>
        [XmlElement]
        public string ProductPirce
        {
            get;
            set;
        }

        /// <summary>
        /// 图片编号
        /// </summary>
        [XmlElement]
        public string ProductImage
        {
            get;
            set;
        }

    }
}
