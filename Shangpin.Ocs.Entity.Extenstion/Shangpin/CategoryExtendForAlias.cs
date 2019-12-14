using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class CategoryExtendForAlias : Category
    {
        /// <summary>
        /// 别名
        /// </summary>
        public string ObjectAlias { get; set; }
        /// <summary>
        /// 层名称
        /// </summary>
        public string CName { get; set; }

        /// <summary>
        /// 别名排序
        /// </summary>
        public string AliasOrder { get; set; }

        public string ObjectNo { get; set; }
        public int KeyID { get; set; }
    }
}
