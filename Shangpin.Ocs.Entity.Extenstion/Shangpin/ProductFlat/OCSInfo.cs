using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ProductFlat
{
    //OCS分类实体
    public class OCSInfo
    {
        public string CategoryNo { get; set; }//分类编号

        public string CategoryName { get; set; }//分类名称

        public string ParentNo { get; set; }//父级分类编号

        public bool isParent { get; set; }//是否为父类

        public string SortUpdateDate { get; set; }//排序修改时间

        public bool IsUpdateDateOne { get; set; }//修改时间是否超过一个月
        public int AutoLastFlag { get; set; }//是否自动沉底
    }
}
