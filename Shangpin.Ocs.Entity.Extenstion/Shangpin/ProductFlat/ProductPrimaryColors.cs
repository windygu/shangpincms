using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ProductFlat
{
    //用来装搜索结果颜色类
    public class ProductPrimaryColors
    {
        public string ColorNO { get; set; }//颜色编号

        public string ColorName { get; set; }//颜色名称

        public string ColorPicFile { get; set; }//颜色图片

        public int ColorProductCount { get; set; }//当前颜色下商品的个数
    }
}
