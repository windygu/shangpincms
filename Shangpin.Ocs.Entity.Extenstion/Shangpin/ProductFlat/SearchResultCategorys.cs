using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ProductFlat
{
    //搜索结果返回的分类类
    public class SearchResultCategorys
    {
        public string CategoryNo { get; set; }//分类编号

        public string CateGoryName { get; set; }//分类名称

        public int CateGoryLevel { get; set; }//分类级别

        public string PrentNo { get; set; }//父类编号

        public int State { get; set; }//分类有效状态

        public int CategorySort { get; set; }//分类排序号

        public int CategoryProductCount { get; set; }//分类商品个数
    }
}
