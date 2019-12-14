using Shangpin.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    /// <summary>
    /// 实体类
    /// </summary>
    public class SWfsBrandIndexInfo : PagingEntityBase
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        //索引编辑时用到
        public short ModuleType { get; set; }
        public string BrandCnName { get; set; }
        public string BrandEnName { get; set; }
        public string BrandLogo { get; set; }
        //public string DateCreate { get; set; }
        //public string ModuleNo { get; set; }
        //SWfsBrandIndex表
        public int IndexId { get; set; }
        public string ModuleNo { get; set; }
        public int TypeId { get; set; }
        public string BrandShowName { get; set; }
        public string BrandNo { get; set; }
        public string BrandPic { get; set; }
        public string BrandAddr { get; set; }
        public int Status { get; set; }
        public int Sort { get; set; }
        public DateTime DateCreate { get; set; }
        public int BrandView { get; set; }
    }
}
