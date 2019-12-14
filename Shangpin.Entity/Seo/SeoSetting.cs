using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Seo
{
    /// <summary>
    /// SEO信息设置
    /// </summary>
    /// 
    /// Date:2012/9/1
    public class SeoSetting
    {
        /// <summary>
        /// 关键词
        /// </summary>
        /// <value>
        /// The key words.
        /// </value>
        /// 
        /// Date:2012/9/1
        public string KeyWords { get; set; }
        /// <summary>
        /// 页面描述
        /// </summary>
        /// <value>
        /// The discription.
        /// </value>
        /// 
        /// Date:2012/9/1
        public string Description { get; set; }
        /// <summary>
        /// 页面标题
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        /// 
        /// Date:2012/9/1
        public string Title { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        /// <typeparam name="string">The type of the tring.</typeparam>
        /// <param name="?">The ?.</param>
        /// <returns></returns>
        /// 
        /// Date:2012/9/1
        public IList<string> Propertys { get; set; }
    }
}

