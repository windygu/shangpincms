using System;

namespace Shangpin.Entity.Item.Brand
{
    /// <summary>
    /// 品牌 展示页面 读取数据信息
    /// </summary>
    public class BrandDetailAuthInfo
    {
        /// <summary>
        ///SWfsBrandTerminal
        /// </summary>
        public string BrandNo { get; set; }
        public string HompageLogoPicNo { get; set; }
        public string HeadLogoPicNo { get; set; }
        public string ConcernLogoPicNo { get; set; }
        public int ConcernNumber { get; set; }
        public string BrandIntroduce { get; set; }
        public short Status { get; set; }
        public short GenderFlag { get; set; }
        public int Sort { get; set; }
        public int IsShowAllGender { get; set; }
        public System.DateTime DateCreate { get; set; }

       /// <summary>
       ///汉字，品牌名称
       /// </summary>
       public String BrandCnName { get; set; }

       /// <summary>
       ///英文 ，品牌名称
       /// </summary>
       public String BrandEnName { get; set; } 
    }
}
