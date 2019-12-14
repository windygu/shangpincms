using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShangPin.Services.Catalog
{
    /// <summary>
    /// 商品列表类型
    /// </summary>
    /// Author:wangtao
    /// Date:2012/7/18
    public struct ProductListType
    {
        /// <summary>
        /// 列表页
        /// </summary>
        /// Author:wangtao
        /// Date:2012/7/18
        public const int Listing = 1;
        /// <summary>
        /// 品牌商品列表页
        /// </summary>
        /// Author:wangtao
        /// Date:2012/7/18
        public const int BrandList = 2;
        /// <summary>
        /// 新品到货
        /// </summary>
        /// Author:wangtao
        /// Date:2012/7/18
        public const int NewArrival = 3;
    }
}
