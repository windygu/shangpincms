using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Shangpin.Entity.Customers
{
    /// <summary>
    /// 上载文件类
    /// </summary>
    public class HttpPostedFile
    {
        public int ContentLength { get; set; }

        public string FileName { get; set; }

        public Stream InputStream { get; set; }

        public string ContentType { get; set; }
    }
}
