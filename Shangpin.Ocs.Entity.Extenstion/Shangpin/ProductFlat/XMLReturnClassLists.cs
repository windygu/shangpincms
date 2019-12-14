using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shangpin.Ocs.Entity.Extenstion.ProductFlat
{
    public class XMLReturnClassLists
    {
        /// <summary>
        /// 产品列表
        /// </summary>
        public List<InterfaceProductInfo> ListProducts = new List<InterfaceProductInfo>();

        /// <summary>
        /// 品牌列表
        /// </summary>
        public List<SearchResultBrands> ListBrands = new List<SearchResultBrands>();

        /// <summary>
        /// 色系列表
        /// </summary>
        public List<ProductPrimaryColors> ListColors = new List<ProductPrimaryColors>();

        /// <summary>
        /// 分类列表
        /// </summary>
        public List<SearchResultCategorys> ListCategorys = new List<SearchResultCategorys>();

        /// <summary>
        /// 产品总记录数
        /// </summary>
        public int docCount { get; set; }//返回的总条数

        /// <summary>
        /// 指定OCS分类下已排序的商品信息
        /// </summary>
        public string SortedProductInfo { get; set; }

        /// <summary>
        /// 已保存的商品数量
        /// </summary>
        public int SaveProductCount { get; set; }
    }
}
