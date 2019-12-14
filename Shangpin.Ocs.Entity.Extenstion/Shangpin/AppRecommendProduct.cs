using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    /// <summary>
    /// App推荐商品实体
    /// </summary>
    public class AppRecommendProductModle
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string Id
        {
            get;
            set;
        }
        /// <summary>
        /// 产品编号
        /// </summary>
        public string ProductNo
        {
            get;
            set;
        }
        /// <summary>
        /// 排序值 （预留）
        /// </summary>
        public string SortId
        {
            get;
            set;
        }
        /// <summary>
        ///创建时间
        /// </summary>
        public string CreateTime
        {
            get;
            set;
        }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator
        {
            get;
            set;
        }
    }
}
