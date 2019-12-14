using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    [Serializable]
    public class VCodeInfo : SWfsVActivityCodesRef
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string MailAddress { get; set; }
        public short UseCodeStatus { get; set; }
        public DateTime BindTime { get; set; }

    }
}
