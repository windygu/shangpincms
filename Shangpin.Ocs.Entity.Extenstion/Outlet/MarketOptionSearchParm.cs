using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Common;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    /// <summary>
    /// 推广管理查询参数
    /// </summary>
   public class MarketOptionSearchParm
    {
       /// <summary>
       /// 活动编号
       /// </summary>
       public string SubjectNoName { get; set; }

       /// <summary>
       /// 活动品牌
       /// </summary>
       public string BrandNo { get; set; }

       public string BrandName { get; set; }

       /// <summary>
       /// 申请开始时间
       /// </summary>
       public string ApplyBeginTime { get; set; }

       /// <summary>
       /// 申请结束时间
       /// </summary>
       public string ApplyEndTime { get; set; }


       /// <summary>
       /// 推广状态 0 1 2
       /// </summary>
       public string SpreadStatus { get; set; }

       /// <summary>
       /// 推广强度1-4
       /// </summary>
       public string Level { get; set; }

       /// <summary>
       /// 分类
       /// </summary>
       public string CategoryNo { get; set; }

       /// <summary>
       /// 活动类型 4专题 5秒杀 1-3普通活动
       /// </summary>
       public string SubjectType { get; set; }

    }
  
}
