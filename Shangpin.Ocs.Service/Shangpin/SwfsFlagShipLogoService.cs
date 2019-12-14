using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class SwfsFlagShipLogoService
    {
        private static SwfsFlagShipLogoService _instance;
        public static SwfsFlagShipLogoService instrance()
        {
            if (_instance == null)
                _instance = new SwfsFlagShipLogoService();
            return _instance;
        }
        public SwfsFlagShipLogo GetEntityById(int id)
        {
            object param = new { LogoId = id };
            return DapperUtil.Query<SwfsFlagShipLogo>("ComBeziWfs_SwfsFlagShipLogo_FetchEntityByIdentity_NoLock", param).FirstOrDefault();
        }
        public SwfsFlagShipLogo GetEntityByBrandNo(string brandNo)
        {
            object param = new { BrandNo = brandNo };
            return DapperUtil.Query<SwfsFlagShipLogo>("ComBeziWfs_SwfsFlagShipLogo_FetchEntityByBrandNo_NoLock", param).FirstOrDefault();
        }
        public int Insert(SwfsFlagShipLogo entity)
        {
            return DapperUtil.Insert<SwfsFlagShipLogo>(entity, true);
        }
        public bool Update(SwfsFlagShipLogo entity)
        {
            return DapperUtil.Update<SwfsFlagShipLogo>(entity);
        }
        public int Delete(string id)
        {
            return DapperUtil.Execute("ComBeziWfs_SwfsFlagShipLogo_FetchEntityByIdentity_NoLock_del", new { AppSlterPicId = id });
        }
    }
}
