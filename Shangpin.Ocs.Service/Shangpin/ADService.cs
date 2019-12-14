using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;

namespace Shangpin.Ocs.Service.Shangpin
{
    /*20131009需求 添加首页AD图管理功能*/
    public class ADService
    {

        #region 获取AD列表
        /// <summary>
        /// 获取AD列表
        /// </summary>
        /// <param name="title"></param>
        /// <param name="datecreate"></param>
        /// <param name="sitNo">尚品1奥莱2</param>
        /// <param name="CategoryName">分类</param>
        /// <returns></returns>
        public IList<WfsCmsContent> GetADList(Dictionary<string,object>dicParam, List<string> positionIds)
        {
            return DapperUtil.Query<WfsCmsContent>("ComBeziWfs_WfsCmsContent_GetWfsCmsContentListBySitNo", dicParam, new { Title = dicParam["Title"], ShowStatus = dicParam["ShowStatus"], DateBegin = dicParam["DateBegin"], DateEnd = dicParam["DateEnd"],
                PositionId =positionIds, PositionParentId = dicParam["PositionParentId"], siteNo = "1" }).ToList();
        }
        #endregion
    }
}
