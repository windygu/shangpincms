using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.Login
{
   public class OcsServiceResult
    {
       /// <summary>
       /// 运行结果
       /// </summary>
       public bool IsSuccess { get; set; }

       /// <summary>
       /// 返回数据内容
       /// </summary>
       public Dictionary<string, object> ContentDic { get; set; }
    }
}
