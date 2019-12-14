using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    /// <summary>
    /// 上新品牌实体
    /// </summary>
    public class SWfsNewBrandInfo : SWfsNewBrandManage
    {
        public string BrandLogo { get; set; }//品牌logo图
        public string BrandEnName { get; set; }//品牌英文名称
        public string BrandCnName { get; set; }//品牌中文名称
        public int NewCount { get; set; }//上新数量 
    }
}
