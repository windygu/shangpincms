using Shangpin.Entity.Wfs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class ChannelRecommendBrandExtends
    {

        public string BrandNO { get; set; }
        public string BrandCnName { get; set; }
        public string BrandEnName { get; set; }
        public string ChannelNO { get; set; }
        public DateTime CreateDate { get; set; }
        public int RecommendBrandID { get; set; }
        public int SortValue { get; set; }
        public string BrandLogo { get; set; }
    }

    public class SWfsCategoryExtends
    {
        public string CategoryName { get; set; }
        public string CategoryNo { get; set; }
        public string ParentNo { get; set; }
        public int Sort { get; set; }
        public short VisibleState { get; set; }
        public int IsRecommend { get; set; }
        public string ChannelNO { get; set; }
        public int RecommendCategoryID { get; set; }
    }

    public class SpHomeRecommendBrandExtends : SWfsSpHomeRecommendBrand
    {
        public string BrandCnName { get; set; }
        public string BrandEnName { get; set; }
        public string BrandLogo { get; set; }
    }

}
