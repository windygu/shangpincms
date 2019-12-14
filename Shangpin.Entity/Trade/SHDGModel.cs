namespace Shangpin.Entity.Trade
{
    public class SHDGModel
    {
        string _u_id = string.Empty;
        /// <summary>
        /// 上海导购网的会员编号
        /// </summary>
        public string u_id
        {
            get
            {
                return _u_id;
            }
            set
            {
                _u_id = value;
            }
        }
        string _target_url = string.Empty;
        /// <summary>
        /// 商家网站的目标链接
        /// </summary>
        public string target_url
        {
            get
            {
                return _target_url;
            }
            set
            {
                _target_url = value;
            }
        }
        string _tracking_code = string.Empty;
        /// <summary>
        /// 效果追踪码
        /// </summary>
        public string tracking_code
        {
            get
            {
                return _tracking_code;
            }
            set
            {
                _tracking_code = value;
            }
        }
        string _shkey = string.Empty;
        /// <summary>
        /// 预付卡验证码
        /// </summary>
        public string shkey
        {
            get
            {
                return _shkey;
            }
            set
            {
                _shkey = value;
            }
        }
        string _username = string.Empty;
        /// <summary>
        ///  用户名称
        /// </summary>
        public string username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }
    }
}
