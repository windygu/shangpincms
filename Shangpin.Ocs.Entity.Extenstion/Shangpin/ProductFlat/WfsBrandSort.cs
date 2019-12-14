using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin.ProductFlat
{
    public class WfsBrandSort
    {
        public string BrandNo { get; set; }//品牌ID

        public string BrandCnName { get; set; }//品牌中文名称

        public string BrandEnName { get; set; }//品牌英文名称

        public string SortUpdateDate { get; set; }//排序修改时间

        public bool IsUpdateDateOne { get; set; }//是否相差一个月

        public int AutoLastFlag { get; set; }//是否自动沉底
    }
}
