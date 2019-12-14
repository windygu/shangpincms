using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    /// <summary>
    /// 流行元素
    /// </summary>
    public class SWfsPopElements
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ElementsId { get; set; }

        /// <summary>
        /// 分类编号 
        /// </summary>
        public string CategoryNo { get; set; }

        /// <summary>
        /// 标签类编号
        /// </summary>
        public string LabelNo { get; set; }

        /// <summary>
        /// 是否显示（0不显示，1显示，默认为0）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 排序ID 
        /// </summary>
        public int SortId { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public string OperatorUserId { get; set; }

        /// <summary>
        /// 创建时间 
        /// </summary>
        public DateTime DateCreate { get; set; }

    }
}
