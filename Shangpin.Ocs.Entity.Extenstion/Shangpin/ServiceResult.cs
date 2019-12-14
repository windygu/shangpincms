using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class ServiceResult
    {
        public IList<string> Errors { get; set; }

        public ServiceResult()
        {
            this.Errors = new List<string>();
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success
        {
            get { return this.Errors.Count == 0; }
        }

        /// <summary>
        /// 错误码
        /// </summary>
        /// <param name="errorCode"></param>
        public void AddErrorCode(string errorCode)
        {
            this.Errors.Add(errorCode);
        }

        /// <summary>
        /// 其他返回数据
        /// </summary>
        public dynamic Result { get; set; }


    }
}
