using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class SwfsFlagShipFlashService
    {
        private static SwfsFlagShipFlashService _instance;
        public static SwfsFlagShipFlashService instrance()
        {
            if (_instance == null)
                _instance = new SwfsFlagShipFlashService();
            return _instance;
        }

        public SwfsFlagShipFlash GetEntityById(int id)
        {
            object param = new { FlashId = id };
            return DapperUtil.Query<SwfsFlagShipFlash>("ComBeziWfs_SwfsFlagShipFlash_FetchEntityByIdentity_NoLock", param).FirstOrDefault();
        }
        public IEnumerable<SwfsFlagShipFlash> GetEntityByBrandNo(string brandNo)
        {
            object param = new { BrandNo = brandNo };
            return DapperUtil.Query<SwfsFlagShipFlash>("ComBeziWfs_SwfsFlagShipFlash_FetchEntityByBrandNo_NoLock", param);
        }

        public IEnumerable<SwfsFlagShipFlash> GetEntityByBranNoAndBeginTime(string brandNo, DateTime BeginDateTime)
        {
            object param = new { brandNo = brandNo, BeginDateTime = BeginDateTime };
            return DapperUtil.Query<SwfsFlagShipFlash>("ComBeziWfs_SwfsFlagShipFlash_FetchEntityByTimeAndBrandNo_NoLock", param);
        }



        public SwfsFlagShipFlash GetEntityByIdAndBrandNo(int id, string brandNo)
        {
            object param = new { FlashId = id, BrandNo = brandNo };
            return DapperUtil.Query<SwfsFlagShipFlash>("ComBeziWfs_SwfsFlagShipFlash_FetchEntityByIDAndBrandNo_NoLock", param).FirstOrDefault();
        }
        public int Insert(SwfsFlagShipFlash entity)
        {
            return DapperUtil.Insert<SwfsFlagShipFlash>(entity, true);
        }
        public bool Update(SwfsFlagShipFlash entity)
        {
            return DapperUtil.Update<SwfsFlagShipFlash>(entity);
        }
        public int Delete(string id)
        {
            return DapperUtil.Execute("ComBeziWfs_SwfsFlagShipFlash_FetchEntityByBrandNo_NoLock_del", new { FlashId = id });
        }

        public IEnumerable<SwfsFlagShipFlash> AlterLists(int pageIndex, int pageSize, Dictionary<string, object> dic, out int count)
        {
            IEnumerable<SwfsFlagShipFlash> list = DapperUtil.Query<SwfsFlagShipFlash>("ComBeziWfs_SwfsFlagShipFlash_GetList", dic, new
            {
                BrandNo = dic["BrandNo"],
                PictureName = dic["PictureName"],
                State = dic["State"],
                PictureIndex = dic["PictureIndex"],
                beginTime = dic["beginTime"] + "",
                endTime = dic["endTime"] + "",
                pageIndex = pageIndex,
                pageSize = pageSize
            }).ToList();
            count = DapperUtil.Query<int>("ComBeziWfs_SwfsFlagShipFlash_GetList_count", dic, new
            {
                BrandNo = dic["BrandNo"],
                PictureName = dic["PictureName"],
                State = dic["State"],
                PictureIndex = dic["PictureIndex"],
                beginTime = dic["beginTime"] + "",
                endTime = dic["endTime"] + ""
            }).First<int>();
            return list;
        }


    }
}
