using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ProductFlat
{
    //用来装搜索结果品牌类
    public class SearchResultBrands
    {
        public string BrandNO { get; set; }//品牌编号

        public string BrandEnName { get; set; }//品牌英文名字

        public string BrandChName { get; set; }//品牌中文名字

        public int ProductCount { get; set; }//当前品牌下当前分类的商品个数
    }
}
