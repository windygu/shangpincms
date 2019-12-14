using System.Collections.Generic;

namespace Shangpin.Entity.Item.Search
{
    /// <summary>
    /// 页面数据JSON对象
    /// </summary>
    /// Author:wangtao
    /// Date:2012/7/17
    public class JsonModel
    {
        public JsonModel()
        { }
        public JsonModel(int tCount, bool nBlock)
        {
            this.NextBlock = nBlock;
            this.Total = tCount;
        }
        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/17
        public int Total { get; set; }
        /// <summary>
        /// is exist next block.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [next block]; otherwise, <c>false</c>.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/17
        public bool NextBlock { get; set; }
        /// <summary>
        /// Gets or sets the product list.
        /// </summary>
        /// <value>
        /// The plist.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/17
        public IList<prd> plist { get; set; }
    }
    /// <summary>
    /// Product
    /// </summary>
    /// Author:wangtao
    /// Date:2012/7/17
    public class prd
    {
        /// <summary>
        /// ProductNo
        /// </summary>
        /// <value>
        /// The no.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/17
        public string no { get; set; }
        /// <summary>
        /// BrandEnName
        /// </summary>
        /// <value>
        /// The b.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/17
        public string b { get; set; }
        /// <summary>
        /// ProductName
        /// </summary>
        /// <value>
        /// The pn.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/17
        public string pn { get; set; }
        /// <summary>
        /// Price(with customer level)
        /// </summary>
        /// <value>
        /// The p.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/17
        public decimal p { get; set; }
        /// <summary>
        /// price in promotion
        /// </summary>
        /// <value>
        /// The pm.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/17
        public decimal pm { get; set; }
        /// <summary>
        /// the product is under promotion ?
        /// </summary>
        /// <value>
        ///   <c>true</c> if ipm; otherwise, <c>false</c>.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/17
        public bool ipm { get; set; }
        /// <summary>
        /// product picture
        /// </summary>
        /// <value>
        /// The img.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/17
        public string img { get; set; }
        /// <summary>
        /// new product .product has new flag?
        /// </summary>
        /// <value>
        ///   <c>true</c> if np; otherwise, <c>false</c>.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/17
        public bool np { get; set; }

        /// <summary>
        /// Is support discount
        /// </summary>
        /// <value>
        ///   <c>true</c> if d; otherwise, <c>false</c>.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/17
        public bool d { get; set; }
        /// <summary>
        /// Gets or sets the product url.
        /// </summary>
        /// <value>
        /// The purl.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/18
        public string purl { get; set; }

        /// <summary>
        /// stock,库存
        /// </summary>
        /// <value>
        /// The st.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/2
        public int st { get; set; }

        /// <summary>
        /// category no.(erp)
        /// </summary>
        /// <value>
        ///   <c>true</c> if s; otherwise, <c>false</c>.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/2
        public string c { get; set; }

        /// <summary>
        /// show products has stock size.
        /// </summary>
        /// <value>
        /// The ss.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/6
        public short ss { get; set; }

        /// <summary>
        /// 产品的体验报告数量
        /// </summary>
        public int eNum { get; set; }

        /// <summary>
        /// 1 标识有组合 0标识没有组合
        /// </summary>
        public string IsGp { get; set; }

        /// <summary>
        /// Discount Rate
        /// </summary>
        public string dr { get; set; }
    }
}
