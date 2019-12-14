using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    /// <summary>
    /// 专卖店实体类列表
    /// </summary>
    public class SWfsBrandExtendList
    {
        public int BrandTypeId { get; set; }
        public int NaviTypeId { get; set; }
        public string BrandNo { get; set; }
        public string BrandEnName { get; set; }
        public string BrandCnName { get; set; }
        public int Status { get; set; }
        public int Status1 { get; set; }
        public string SpecialityStorePic { get; set; }
    }
}
