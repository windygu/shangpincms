using System;

namespace Shangpin.Entity.Item.Brand
{
    /// <summary>
    /// 品牌 读取数据信息
    /// </summary>
   public  class BrandListInfo
    {
        /// <summary>
        ///brand no
        /// </summary>
       public String BrandNo { get; set; }

       /// <summary>
       ///汉字，品牌名称
       /// </summary>
       public String BrandCnName { get; set; }

       /// <summary>
       ///英文 ，品牌名称
       /// </summary>
       public String BrandEnName { get; set; }

       /// <summary>
       ///女士品牌状态（默认为0）0：女士待创建品牌终端页；1：女士已创建品牌终端页；2：女士已发布品牌终端页。
       /// </summary>
       public  Int16 Status { get; set; }
      
       /// <summary>
       /// 男士品牌故事（默认为0）0：男士待创建品牌终端页；1：男士已创建品牌终端页；2：男士已发布品牌终端页。
       /// </summary>
       public Int16 MStatus { get; set; }

       // <summary>
       /// 英文首字母
       /// </summary>
       public String EnFristLetter { get; set; }
       /// <summary>
       /// 汉字首字母
       /// </summary>
       public String ZhFristLetter { get; set; }

       /// <summary>
       ///男 授权品牌终端页表 是否存在
       /// </summary>
       public int? MenAuthorized { get; set; }

       /// <summary>
       /// 女 授权品牌终端页表 是否存在
       /// </summary>
       public int? WomenAuthorized { get; set; }

       /// <summary>
       /// 女  商品和前端品类表的关系 产品数量
       /// </summary>
       public int? WomenCount { get; set; }
       /// <summary>
       /// 男  商品和前端品类表的关系 产品数量
       /// </summary>
       public int? MenCount { get; set; }
       /// <summary>
       /// 总产品量
       /// </summary>
       public int? ProductCount { get; set; }

       /// <summary>
       /// Gender 男 女 中性
       /// </summary>
       public Int16 Gender { get; set; } 
    }
}
