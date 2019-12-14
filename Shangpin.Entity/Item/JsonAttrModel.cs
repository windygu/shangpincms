using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Item
{
    /// <summary>
    /// 商品属性json对象
    /// </summary>
    /// Author:wangtao
    /// Date:2012/7/20
    public class JsonAttrModel
    {
        /// <summary>
        /// ProductNo
        /// </summary>
        /// <value>
        /// The no.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/20
        public string no { get; set; }
        /// <summary>
        /// Product MinorAttr type name
        /// </summary>
        /// <value>
        /// The attrname.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/20
        public string ccname { get; set; }
        /// <summary>
        /// product image list
        /// </summary>
        /// <value>
        /// The imglist.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/20
        public IList<pimg> imglist { get; set; }
        /// <summary>
        /// ProductMinorAttr list
        /// </summary>
        /// <value>
        /// The attrlist.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/20
        public IList<attr> cclist { get; set; }
    }
    /// <summary>
    /// 商品图片
    /// </summary>
    /// Author:wangtao
    /// Date:2012/7/20
    public class pimg
    {
        /// <summary>
        /// 缩略图
        /// </summary>
        /// <value>
        /// The simg.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/20
        public string simg { get; set; }
        /// <summary>
        /// 预览图
        /// </summary>
        /// <value>
        /// The mimg.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/20
        public string mimg { get; set; }
        /// <summary>
        /// 放大图
        /// </summary>
        /// <value>
        /// The bimg.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/20
        public string bimg { get; set; }
    }
    /// <summary>
    /// 次要属性
    /// </summary>
    /// Author:wangtao
    /// Date:2012/7/20
    public class attr
    {
        /// <summary>
        /// 属性值
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/20
        public string value { get; set; }

        /// <summary>
        /// 国际码
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/20
        public string size { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/20
        public string name { get; set; }
        /// <summary>
        /// 属性转换标题
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/20
        public string title { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        /// <value>
        /// The q.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/20
        public int q { get; set; }
    }
}
