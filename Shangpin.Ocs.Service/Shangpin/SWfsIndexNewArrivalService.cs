using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsIndexNewArrivalService
    {
        /// <summary>
        /// 执行添加上新信息
        /// </summary>
        /// <param name="sWfsIndexNewArrival">SWfsIndexNewArrival实体</param>
        /// <returns>返回主键id</returns>
        public int AddSWfsIndexNewArrival(SWfsIndexNewArrival sWfsIndexNewArrival)
        {
            return DapperUtil.Insert<SWfsIndexNewArrival>(sWfsIndexNewArrival,true);
        }
        
        /// <summary>
        /// 获取全部的上新信息
        /// </summary>
        /// <returns></returns>
        public List<SWfsIndexNewArrivalExt> SelAllSWfsIndexNewArrival(int pageindex,int pageSize,out int total)
        {
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsIndexNewArrival_SelCount").FirstOrDefault();

            return DapperUtil.Query<SWfsIndexNewArrivalExt>("ComBeziWfs_SWfsIndexNewArrival_SelAll", new { pageIndex = pageindex, pageSize = pageSize}).ToList();
        }


        /// <summary>
        /// 获取全部的商品信息
        /// </summary>
        /// <param name="newArrivalId">上新的主键id</param>
        /// <returns></returns>
        public List<Product> SelAllSWfsIndexNewArrivalProduct(int newArrivalId)
        {
            

            return DapperUtil.Query<Product>("ComBeziWfs_SWfsIndexNewArrival_SelAllProduct", new { NewArrivalId = newArrivalId }).ToList();
        }

        /// <summary>
        /// 查询单个上新信息
        /// </summary>
        /// <param name="pkid">上新id</param>
        /// <returns></returns>
        public SWfsIndexNewArrival SelSWfsIndexNewArrivalProductByIdentity(int pkid)
        {
            return DapperUtil.Query<SWfsIndexNewArrival>("ComBeziWfs_SWfsIndexNewArrival_SelOneByIdentity", new { NewArrivalId = pkid }).FirstOrDefault();

        }

        /// <summary>
        /// 修改上新信息
        /// </summary>
        /// <param name="pkid">上新实体</param>
        /// <returns></returns>
        public int UpdateSWfsIndexNewArrivalManager(SWfsIndexNewArrival sWfsIndexNewArrival)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsIndexNewArrival_UpdateManager", new
            {
                NewArrivalTitle = sWfsIndexNewArrival.NewArrivalTitle,
                StartDate = sWfsIndexNewArrival.StartDate,
                UpdateOperateUserId = sWfsIndexNewArrival.UpdateOperateUserId,
                UpdateDate=DateTime.Now,
                NewArrivalId = sWfsIndexNewArrival.NewArrivalId,
            });
        }

        /// <summary>
        /// 删除上新信息
        /// </summary>
        /// <param name="pkid">上新主键编号</param>
        /// <returns></returns>
        public int DelSWfsIndexNewArrivalStatus(int pkid)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsIndexNewArrival_DelNewComming", new
            {
                NewArrivalId = pkid,
            });
        }

        /// <summary>
        /// 判断添加的上新是否有重复时间的
        /// </summary>
        /// <param name="startdate">上新时间</param>
        /// <returns></returns>
        public int SelSWfsIndexNewArrivalDatailDate(DateTime startdate, int newArrivalId)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsIndexNewArrival_SelDetailDate", new { StartDate = startdate, NewArrivalId = newArrivalId }).FirstOrDefault();
        }
    }
}
