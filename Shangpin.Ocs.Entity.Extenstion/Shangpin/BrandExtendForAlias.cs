using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class BrandExtendForAlias : SpBrand
    {
        public string ObjectAlias { get; set; }
        public string AliasOrder { get; set; }
        public string ObjectNo { get; set; }
        public int KeyID { get; set; }
    }
}
