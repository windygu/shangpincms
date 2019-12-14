using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsPopElementsService
    {
        /// <summary>
        /// 获取全部的流行元素
        /// </summary>
        /// <returns></returns>
        public List<SWfsPopElements> GetSWfsPopElementsList()
        {
            return DapperUtil.Query<SWfsPopElements>("ComBeziWfs_SWfsPopElements").ToList();
        }
    }
}
