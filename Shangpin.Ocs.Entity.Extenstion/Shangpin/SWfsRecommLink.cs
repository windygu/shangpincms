using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{

    /// <summary>
    /// 热门推荐
    /// </summary>
    public class SWfsRecommLink
    {
        /// <summary>
        /// 主键 
        /// </summary>
        public int LinkId{get;set;}

        /// <summary>
        /// 分类编号 
        /// </summary>
        public string CategoryNo{get;set;}      

        /// <summary>
        /// 中文名称 
        /// </summary>
        public string LinkName{get;set;}

        /// <summary>
        /// 位置（头部=1,列表=2，默认为0）
        /// </summary>
        public int PagePosition{get;set;}

        /// <summary>
        /// 链接地址 
        /// </summary>
        public string LinkAddress{get;set;}
 
        /// <summary>
        /// 打开方式（_blank,_self） 
        /// </summary>
        public string LinkTarget{get;set;}

        /// <summary>
        /// 开始时间 
        /// </summary>
        public DateTime BeginTime{get;set;}
 
        /// <summary>
        /// 结束时间 
        /// </summary>
        public DateTime EndTime{get;set;}

        /// <summary>
        /// 排序ID
        /// </summary>
        public int SortId{get;set;}

        /// <summary>
        /// 显示状态（1、显示，0隐藏，默认为：0） 
        /// </summary>
        public int Status{get;set;}

        /// <summary>
        /// 创建时间 
        /// </summary>
        public DateTime DateCreate{get;set;}

        /// <summary>
        /// 操作人ID 
        /// </summary>
        public string OperatorUserId { get; set; }

 
 

    }
}
