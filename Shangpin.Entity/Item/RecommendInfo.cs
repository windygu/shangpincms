using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Item
{
    public class RecommendInfo
    {
        /// <summary>
        /// 搭配组合编号 - WfsStyleGroup
        /// </summary>
        public string StyleGroupNo { get; set; }
        /// <summary>
        /// 搭配组合详情图片编号 -- WfsStyleGroupDetail
        /// </summary>
        public string StyleGroupDetailPicNo { get; set; }
        public short PicType { get; set; }
        public string ProductNo { get; set; }
        public int PictureOrder { get; set; }

        /// <summary>
        /// 品牌信息
        /// </summary>
        public string BrandEnName { get; set; }
        public string BrandCnName { get; set; }

        /// <summary>
        /// 商品信息
        /// </summary>
        public decimal LimitedPrice { get; set; }
        public decimal SellPrice { get; set; }
        public decimal PlatinumPrice { get; set; }
        public decimal DiamondPrice { get; set; }
        public string ProductName { get; set; }
        public short GenderStyle { get; set; }
    }
}
