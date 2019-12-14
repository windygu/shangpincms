using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    /// <summary>
    /// 分组商品视图排序页
    /// </summary>
    public enum SubjectSortRuleType
    {
        /// <summary>
        /// 按价格
        /// </summary>
        price = 1,

        /// <summary>
        /// 按品牌
        /// </summary>
        brand = 2,

        /// <summary>
        /// 按库存
        /// </summary>
        stock = 3,

        /// <summary>
        /// 按收藏数
        /// </summary>
        collect = 4,

        /// <summary>
        /// 按上架时间
        /// </summary>
        putawaytime = 5,

        /// <summary>
        /// 按折扣
        /// </summary>
        discount = 6,

        /// <summary>
        /// 按品类
        /// </summary>
        category = 7,

        /// <summary>
        /// 按色系
        /// </summary>
        color = 8,

        /// <summary>
        /// 默认规则
        /// </summary>
        _default = 9
    }
}
