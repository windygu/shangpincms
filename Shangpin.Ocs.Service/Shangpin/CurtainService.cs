using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Framework.Common;
using Shangpin.Entity.Wfs;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class CurtainService
    {
        //删除一条数据
        public int CurtainDelete(int curtainId)
        {
            return DapperUtil.Execute("ComBeziWfs_WfsCmsContent_SWfsCurtain_delete", new { CurtainId = curtainId });
        }
        //修改状态
        public bool CurtainStatus(int curtainId, int curtainStatus)
        {
            return DapperUtil.UpdatePartialColumns<SWfsCurtain>(new { CurtainId = curtainId, CurtainStatus = curtainStatus });
        }
        //添加
        public int CurtainCreate(SWfsCurtain obj)
        {
            return DapperUtil.Insert<SWfsCurtain>(obj, true);
        }
        //修改
        public bool SWfsCurtainUpdate(SWfsCurtain obj)
        {
            return DapperUtil.Update<SWfsCurtain>(obj);
        }
        public SWfsCurtain CurtainListId(int curtainId)
        {
            return DapperUtil.Query<SWfsCurtain>("ComBeziWfs_WfsCmsContent_SWfsCurtain_ID", new { CurtainId = curtainId }).FirstOrDefault();
        }
    }
}
