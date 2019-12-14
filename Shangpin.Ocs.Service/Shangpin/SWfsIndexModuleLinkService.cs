using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Ocs.Entity.Extenstion.Login;
using Shangpin.Ocs.Service.Common;
using Shangpin.Entity.ComBeziPic;
using Shangpin.Ocs.Service.Outlet;
using System.IO;
using System.Web;
namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsIndexModuleLinkService
    {
        /// <summary>
        /// 获取楼层连接
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public List<SWfsIndexModuleLink> GetSWfsIndexModuleLinkByModuleId(int moduleId)
        {
            if (moduleId <= 0) return new List<SWfsIndexModuleLink>();
            List<SWfsIndexModuleLink> links = DapperUtil.Query<SWfsIndexModuleLink>("ComBeziWfs_SWfsIndexModuleLink_GetSWfsIndexModuleLinkByModuleId", new { ModuleId = moduleId }).FilterList();

            return links ?? new List<SWfsIndexModuleLink>();
        }
        /// <summary>
        /// 更新或添加楼层连接
        /// </summary>
        /// <param name="links"></param>
        public void InsertOrUpdateFloorSWfsIndexModuleLink(List<SWfsIndexModuleLink> links)
        { 
            foreach (SWfsIndexModuleLink item in links)
            {
                if (item.LinkId > 0)
                    DapperUtil.UpdatePartialColumns<SWfsIndexModuleLink>(item);
                else
                    DapperUtil.Insert<SWfsIndexModuleLink>(item);
            }
        }
        /// <summary>
        /// 根据链接ID获取链接对象
        /// </summary>
        /// <param name="LinkId"></param>
        /// <returns></returns>
        public SWfsIndexModuleLink GetSwfsIndexModuleLinkByLinkId(int LinkId)
        {
            if (LinkId <= 0) return null;
            return DapperUtil.Query<SWfsIndexModuleLink>("ComBeziWfs_SwfsIndexModuleLink_GetSwfsIndexModuleLinkByLinkId", new { LinkId }).FirstOrDefault();
        }
        /// <summary>
        /// 插入或更新link
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public int InsertOrUpdateLink(ref SWfsIndexModuleLink link, Boolean needEntity = false)
        {
            try
            {

                if (link.LinkId <= 0)
                {
                    DapperUtil.Insert<SWfsIndexModuleLink>(link);
                }
                else
                {
                    DapperUtil.Update<SWfsIndexModuleLink>(link);
                }
                if (needEntity)
                {
                    link = DapperUtil.Query<SWfsIndexModuleLink>("ComBeziWfs_SWfsIndexModuleLink_GetSWfsIndexModuleLinkByModuleIdAndDateCreate", new { ModuleId = link.ModuleId, link.DateCreate }).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return -1;
            }
            return 1;
        }
    }
}
