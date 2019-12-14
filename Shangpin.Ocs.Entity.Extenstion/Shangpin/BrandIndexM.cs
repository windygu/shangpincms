using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Common;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    [Serializable]
    public class BrandIndexM : PagingEntityBase
    {
        public int IndexId { get; set; }
        public string ModuleNo { get; set; }
        public string BrandAddr { get; set; }
        public string BrandNo { get; set; }
        public string BrandPic { get; set; }
        public string BrandShowName { get; set; }
        public int BrandView { get; set; }
        public DateTime DateCreate { get; set; }
        public int Sort { get; set; }
        public int Status { get; set; }
        public int TypeId { get; set; }
        public string BrandLogo { get; set; }
    }
}
