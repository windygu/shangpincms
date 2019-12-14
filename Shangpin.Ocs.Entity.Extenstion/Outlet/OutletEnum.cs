using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    public enum SubjectType
    {
        /// <summary>
        /// 所有活动
        /// </summary>
        AllSubject = 0,
        /// <summary>
        /// 今天的活动
        /// </summary>
        ToDaySubject = 1,
        /// <summary>
        /// 即将结束的活动
        /// </summary>
        AboutExpireSubject = 2,
        /// <summary>
        /// 即将开启的活动
        /// </summary>
        AboutBeginSubject = 3,
        /// <summary>
        /// 于今天开始，但当前时刻还未开始的活动
        /// </summary>
        NotStartedToDay,
        /// <summary>
        /// 正在进行
        /// </summary>
        SaleingToDaySubject
    }
}
