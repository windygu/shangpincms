using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class SWfsPromotionsList
    {
        //SWfsProductPromotionTip
        public int PromotionInfoId { get; set; }
        public string PromotionInfoName { get; set; }
        public string MemberSet { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUsterId { get; set; }
        public int IsAllProduct { get; set; }
        public string TipContent { get; set; }
        //SWfsProductPromotionTipRef
        public int ProductPromotionRefID { get; set; }
        public string ProductNo { get; set; }
    }
}
