using System;

namespace Shangpin.Entity.Common
{
    [Serializable]
    public class SSOLoginInfo
    {
        /// <summary>
        /// 用户ID号
        /// </summary>
        public string UserId { get; set; }
        
        /// <summary>
        /// 登录名称
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 不否自动登录
        /// </summary>
        public bool IsRememberMe { get; set; }

        /// <summary>
        /// 当前网站(1：shangpin,2：outlets)
        /// </summary>
        public int CurrentSite { get; set; }
    }
}
