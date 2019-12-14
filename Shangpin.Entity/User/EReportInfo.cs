using System;
using System.Collections.Generic;
using Shangpin.Entity.Wfs;

namespace Shangpin.Entity.User
{
    public class EReportInfo
    {
        /// <summary>
        /// 商品图片
        /// </summary>
        public string ProductPic { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductNo { get; set; }
        /// <summary>
        /// 试穿尺码
        /// </summary>
        public string ProductSize { get; set; }
        /// <summary>
        /// 体验报告内容
        /// </summary>
        public string ExperienceReportContent { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime  PublishTime { get; set; }
        /// <summary>
        /// 体验报告编号
        /// </summary>
        public Int32 ExperienceReportID { get; set; }
        /// <summary>
        /// 搭配图
        /// </summary>
        public IList<SWfsExperienceReportPic> ExperienceReportPics { get; set; }
        /// <summary>
        /// 品牌英文名称
        /// </summary>
        public string BrandEnName { get; set; }
        /// <summary>
        /// 品牌中文名称
        /// </summary>
        public string BrandCnName { get; set; }


    }
}
