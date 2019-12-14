using System.Collections.Generic;

namespace Shangpin.Entity.Item.Brand
{
    public class BrandContentModel
    {
        /// <summary>
        ///  品牌索引页面中英文切换数据
        /// </summary>
        /// <returns></returns>
        public List<BrandListInfo> BrandListContent { get; set; }

        /// <summary>
        /// 品牌索引页面总条数
        /// </summary>
        /// <returns></returns>
        public int BrandRowList { get; set; }

        /// <summary>
        /// 品牌索引页面分行数(每行几个首字母)(现在用列来排)
        /// </summary>
        /// <returns></returns>
        public int BrandRowCount { get; set; }
        
        /// <summary>
        /// 品牌显示中英文信息
        /// </summary>
        /// <returns></returns>
        public List<BrandIndexInfo> BrandIndexInfoRow { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        /// <returns></returns>
        public int BrandGender { get; set; }
        /// <summary>
        /// 中英文切分选项（0中文，1英文）
        /// </summary>
        /// <returns></returns>
        public int BrandSiteNo { get; set; }
        
    }
}