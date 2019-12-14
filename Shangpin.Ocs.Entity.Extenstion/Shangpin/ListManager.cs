using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    /// <summary>
    /// 列表页管理
    /// </summary>
    public class ListManager
    {
        /// <summary>
        /// 热门推荐
        /// </summary>
        public IList<SWfsRecommLink> ReCommLinkList
        {
            set;
            get;
        }

        /// <summary>
        /// 产品标签列表
        /// </summary>
        public IList<ProductLabel> ProductLabelList
        {
            set;
            get;
        }


    }
}
