using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
namespace Shangpin.Ocs.Service.Shangpin
{
    public class VsinsService
    {
        public List<VsinsHotProduct> SelectHotProducts(Dictionary<string,object> dicStr,int pageindex,int pagesize)
        {
            return DapperUtil.Query<VsinsHotProduct>("ComBeziWfs_SWfsHotProduct_SelectAll", dicStr, new { ProductNo = dicStr["ProductNo"].ToString(), SelectTime = dicStr["SelectTime"].ToString(), pageIndex = pageindex, pageSize = pagesize }).ToList();
        }
    }
}
