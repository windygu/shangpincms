using Shangpin.Framework.Common;
using System;
using System.Collections.Generic;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsFlagShipOperationPictureService
    {
        private static SWfsFlagShipOperationPictureService _instance;
        public static SWfsFlagShipOperationPictureService instance()
        {
            if (_instance == null)
                _instance = new SWfsFlagShipOperationPictureService();
            return _instance;
        }
        public int Insert(SWfsFlagShipOperationPicture entity)
        {
            return DapperUtil.Insert<SWfsFlagShipOperationPicture>(entity, true);
        }
        public bool Update(SWfsFlagShipOperationPicture entity)
        {
            return DapperUtil.Update<SWfsFlagShipOperationPicture>(entity);
        }
        public int Delete(string id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsFlagShipOperationPicture_Del", new { PictureManageId = id });
        }
        public IEnumerable<SWfsFlagShipOperationPicture> GetEntityByID(int id)
        {
            return DapperUtil.Query<SWfsFlagShipOperationPicture>("SWfsFlagShipOperationPicture", new { PictureManageId = id });
        }
        public IEnumerable<SWfsFlagShipOperationPicture> GetEntityByBrandNo(string BrandNo)
        {
            return DapperUtil.Query<SWfsFlagShipOperationPicture>("ComBeziWfs_SWfsFlagShipOperationPicture_FetchEntityByBrandNo_NoLock", new { BrandNo = BrandNo });
        }
        public SWfsFlagShipOperationPicture GetEntityByBrandNoAndIndex(string BrandNo, int PictureIndex)
        {
            return DapperUtil.Query<SWfsFlagShipOperationPicture>("ComBeziWfs_SWfsFlagShipOperationPicture_FetchEntityByBrandNoAndIndex_NoLock", new { BrandNo = BrandNo, PictureIndex = PictureIndex }).FirstOrDefault();
        }

    }
}
