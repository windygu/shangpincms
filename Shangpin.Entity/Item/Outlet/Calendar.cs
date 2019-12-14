using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Item.Outlet
{
    /// <summary>
    /// 预售日历实体类 原项目的实体类名称是：（Calendar ）
    /// </summary>
    public class CalendarInfo
    {
        /// <summary>
        /// 星期几
        /// </summary>
        public string DayOfWeek { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public string Month { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string Day { get; set; }
    }
}
