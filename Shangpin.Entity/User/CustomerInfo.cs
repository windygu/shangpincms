using System;

namespace Shangpin.Entity.User
{
    /// <summary>
    /// 用于存储用户会话信息，如等级，姓名等
    /// </summary>
    [Serializable]
    public class CustomerInfo
    {
        /// <summary>
        /// 用户ID号
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 等级编码
        /// </summary>
        public string LevelNo { get; set; }

        /// <summary>
        /// 等级名称
        /// </summary>
        public string LevelName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        private short? _gender;

        public short? Gender
        {
            get
            {
                if (!_gender.HasValue)
                    return 0;
                if (_gender.Value != 0 && _gender.Value != 1)
                    _gender = 0;

                return _gender;
            }
            set { _gender = value; }
        }

        /// <summary>
        /// 性别
        /// </summary>
        public string GenderName
        {
            get
            {
                if (Gender == 0)
                    return "女士";
                if (Gender == 1)
                    return "先生";
                return string.Empty;
            }
        }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        public short Status { get; set; }

        public string Password { get; set; }

        public int ErrorCount { get; set; }

        public string SecuritySalt { get; set; }
        /// <summary>
        /// 邮箱@前部分
        /// </summary>
        public string EmailPrefix
        {
            get
            {
                if (string.IsNullOrEmpty(this.Email))
                    return "";
                var idx = Email.IndexOf('@');
                return idx < 0 ? "" : Email.Substring(0, idx);
            }
        }


        /// <summary>
        /// 登录日期
        /// </summary>
        public DateTime LoginTime { get; set; }


        public int UserFrom { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string TrueName { get; set; }


        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }


        /// <summary>
        /// Email 是否验证
        /// </summary>
        public short IsEmailVerified { get; set; }

        /// <summary>
        /// mobile 是否验证
        /// </summary>
        public short IsMobileVerified { get; set; }

        public decimal ConsumedAmount { get; set; }
        public decimal AolaiAmount { get; set; }
        public decimal ShangpinAmount { get; set; }
        public decimal DiffAmount { get; set; }
        public string UpLevelName { get; set; }
        public string BeActivedLevelNo { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        public string InviteCode { get; set; }

        private string _sessionId = string.Empty;


        public string SessionId
        {
            get
            {
                if (string.IsNullOrEmpty(_sessionId))
                    return new Guid().ToString("N");
                return _sessionId;
            }
            set { _sessionId = value; }
        }

        public bool IsLevelActived { get; set; }

        /// <summary>
        /// 标示用户当前在尚品还是奥莱
        /// </summary>
        public SiteNo CurrentSite { get; set; }

        /// <summary>
        /// 支付宝密钥
        /// </summary>
        public string AliToken { get; set; }

        private string _trueEMail;

        public string TrueEmail
        {
            get
            {
                if (string.IsNullOrEmpty(_trueEMail))
                {
                    return Email;
                }
                else
                    return _trueEMail;
            }
            set { _trueEMail = value; }
        }

        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// 尚品 outlet
    /// </summary>
    public enum SiteNo
    {
        /// <summary>
        /// 尚品为1
        /// </summary>
        ShangPin = 1,
        /// <summary>
        /// 奥莱为2
        /// </summary>
        Outlet = 2
    }
    public class UserLevelInfo
    { 
        public string UserId {get;set;}
        public short Type { get; set; }
        public short Status { get; set; }
        public DateTime LevelEndDate { get; set; }
    }
}
