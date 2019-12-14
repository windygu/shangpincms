using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Common;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    [Serializable]
    public class SWfsStyleActivityPicM : PagingEntityBase
    {

        public string ActivityName { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUserId { get; set; }
        public string PicNo { get; set; }
        public string PicUrl { get; set; }
        public int SAID { get; set; }
        public DateTime StartTime { get; set; }
    }
}
