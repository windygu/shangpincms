using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Item
{
   public class ProductItemCode
    {
       /// <summary>
       /// 输出参数
       /// </summary>
       public string StandarName { get; set; }

       /// <summary>
       /// 输出参数
       /// </summary>
       public string InternatinalSize { get; set; }

       /// <summary>
       /// 输入
       /// </summary>
       public string CategoryNo { get; set; }
       /// <summary>
       /// 输入
       /// </summary>
       public int SizeStandard { get; set; }
       /// <summary>
       /// 输入
       /// </summary>
       public string Size { get; set; }

       /// <summary>
       /// 暂时用于奥莱尺码转换 以后尚品会转换！
       /// </summary>
       public string SizeType { get; set; }
    }
}
