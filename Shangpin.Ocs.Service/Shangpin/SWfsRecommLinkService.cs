using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsRecommLinkService
    {
        /// <summary>
        /// 获取全部的热门推荐
        /// </summary>
        /// <returns></returns>
        public List<SWfsRecommLink> GetSWfsRecommLinkList(string categoryNo)
        {
            return DapperUtil.Query<SWfsRecommLink>("ComBeziWfs_SWfsRecommLink", new { CategoryNo = categoryNo }).ToList();
        }

        /// <summary>
        /// 添加热门推荐
        /// </summary>
        /// <returns></returns>
        public int AddSWfsRecommLink(SWfsRecommLink sWfsRecommLink)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsRecommLink_Add", new
            {
                @CategoryNo = sWfsRecommLink.CategoryNo,
                @LinkName = sWfsRecommLink.LinkName,
                @PagePosition = sWfsRecommLink.PagePosition,
                @LinkAddress = sWfsRecommLink.LinkAddress,
                @LinkTarget = sWfsRecommLink.LinkTarget,
                @BeginTime = sWfsRecommLink.BeginTime,
                @EndTime = sWfsRecommLink.EndTime,
                @SortId = sWfsRecommLink.SortId,
                @Status = sWfsRecommLink.Status,
                @OperatorUserId = sWfsRecommLink.OperatorUserId

            });
        }

        /// <summary>
        /// 重新排序
        /// </summary>
        /// <param name="linkid">主键id</param>
        /// <param name="sortid">排序数据</param>
        /// <returns></returns>
        public int UpdateSortIdSWfsRecommLink(string linkid, string sortid)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsRecommLink_UpdateSortId", new { SortId = sortid, LinkId = linkid });
        }


        /// <summary>
        /// 删除推荐信息
        /// </summary>
        /// <param name="linkId">主键id</param>
        /// <returns></returns>
        public int DeleteSWfsRecommLinkByLinkId(string linkId)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsRecommLink_Delete", new { LinkId = linkId });
        }


        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="linkid">主键id</param>
        /// <param name="sortid">状态</param>
        /// <returns></returns>
        public int UpdateStatusSWfsRecommLink(string linkid, string status)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsRecommLink_UpdataSatate", new { Status = status, LinkId = linkid });
        }

        /// <summary>
        /// 获取热门推荐的最大SortId
        /// </summary>
        /// <param name="categoryNo"></param>
        /// <returns></returns>
        public int GetSWfsRecommLinkSortByCategoryNo(string categoryNo)
        {
            int SortId = 0;
            List<SWfsRecommLink> list = DapperUtil.Query<SWfsRecommLink>("ComBeziWfs_SWfsRecommLink_GetMaxSortByCategoryNo", new { CategoryNo = categoryNo }).ToList();
            if (list != null && list.Count > 0)
            {
                SortId = list[0].SortId + 1;
            }
            return SortId;
        }

    }
}
