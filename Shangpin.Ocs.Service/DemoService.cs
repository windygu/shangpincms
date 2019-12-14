using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;

namespace Shangpin.Ocs.Service
{
   public  class DemoService
    {
        public WfsProduct   GetOneProduct()
        {
            return DapperUtil.QueryByIdentityWithNoLock<WfsProduct>("0100726");
        }
    }
}
