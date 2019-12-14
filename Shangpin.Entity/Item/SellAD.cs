using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Item
{
    public class SellAD
    {
        /// <summary>
        /// 销售广告
        /// </summary>
        public List<ADImg> OnSellADList { get; set; }

        /// <summary>
        /// 预售广告
        /// </summary>
        public List<ADImg> PreSellADList { get; set; }        
    }
    
    /// <summary>
    /// 广告图片
    /// </summary>
    public class ADImg
    {
        /// <summary>
        /// 预售名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 通栏图片
        /// </summary>
        public string FullPic { get; set; }

        /// <summary>
        /// 半通栏图片
        /// </summary>
        public string HalfPic { get; set; }

        /// <summary>
        /// 预售图片链接地址
        /// </summary>
        public string PicLinkUrl { get; set; }
    }

}
