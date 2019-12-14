using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.ComBeziPicLab;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    /// <summary>
    /// 标签主表扩展
    /// </summary>
    public class T_tag_baseExtenstion : t_tag_base
    {
        /// <summary>
        /// 关联的搭配数量
        /// </summary>
        public int relCount { get; set; }
    }
}
