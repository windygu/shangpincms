using System;

namespace Shangpin.Entity.Item.Brand
{
    /// <summary>
    /// 品牌 读取数据信息
    /// </summary>
    public class CategoryNoInfo
    {  
        /// <summary>
        /// CategoryNo 分类号
        /// </summary>
       public String CategoryNo { get; set; }

       /// <summary>
       ///CategoryName 分类名称
       /// </summary>
       public String CategoryName { get; set; }

       /// <summary>
       ///ProductCategoryNo 分类号
       /// </summary>
       public String ProductCategoryNo { get; set; }
       /// <summary>
       ///性别
       /// </summary>
       public int? ProductCategoryGender { get; set; } 
    }
}
