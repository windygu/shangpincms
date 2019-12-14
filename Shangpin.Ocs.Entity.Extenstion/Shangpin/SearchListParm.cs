using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    /// <summary>
    /// （男女）列表页,品牌列表页产品搜索查询参数
    /// </summary>
    public class SearchOCSParm
    {

        public string OCSNO { get; set; }
        public string OCSBrandNO { get; set; }
        public string OCSName { get; set; }

    }


    /// <summary>
    /// 搜索结果
    /// </summary>
    /// Author:wangtao
    /// Date:2012/7/10
    public class SearchResultInfo
    {
        /// <summary>
        /// 搜索响应信息
        /// </summary>
        /// <value>
        /// The search response info.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/10
        public ResponseHeader ResponseHeaderInfo { get; set; }
        /// <summary>
        /// 搜索返回商品列表
        /// </summary>
        /// <value>
        /// The product list.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/10
        public IList<OCSInfo> ProductListInfo { get; set; }

        public IList<string> RalatedKeyWordInfo { get; set; }
    }

    /// <summary>
    /// 搜索响应头部
    /// </summary>
    /// Author:wangtao
    /// Date:2012/7/10
    public class ResponseHeader
    {
        public int Status { get; set; }
        public decimal QTime { get; set; }
        public int Start { get; set; }
        public int NumFound { get; set; }
    }

    public class OCSInfo
    {
        public string OCSNO { get; set; }
        public string OCSName { get; set; }
        public string OCSParentID { get; set; }
        public int OCSLevel { get; set; }
        public int Gender { get; set; }
        public string BrandNO { get; set; }
        public int ChildCount { get; set; }
        public bool isAdd { get; set; }
        public bool isParent { get; set; }
        public bool isVisible { get; set; }
    }
}
