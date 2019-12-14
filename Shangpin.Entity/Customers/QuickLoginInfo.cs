using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Customers
{
    public class QuickLoginInfo
    {
        #region 属性

        /// <summary>
        /// 用户邮箱
        /// </summary>
        private string _email;
        public string Email { get { return _email; } set { _email = value; } }

        /// <summary>
        /// 登录名 , 有可能是邮箱或Email , 一般用来生成虚拟账号
        /// </summary>
        private string _loginname;
        public string LoginName { get { return _loginname; } set { _loginname = value; } }

        /// <summary>
        /// 性别
        /// </summary>
        private string _gender;
        public string Gender { get { return _gender; } set { _gender = value; } }

        /// <summary>
        /// IP地址
        /// </summary>
        private string _ip;
        public string Ip { get { return _ip; } set { _ip = value; } }

        /// <summary>
        /// 真实姓名
        /// </summary>
        private string _realname;
        public string RealName { get { return _realname; } set { _realname = value; } }

        /// <summary>
        /// 手机号
        /// </summary>
        private string _mobile;
        public string Mobile { get { return _mobile; } set { _mobile = value; } }

        private string _activatecode;
        /// <summary>
        /// 邀请码
        /// </summary>
        public string ActivateCode { get { return _activatecode; } set { _activatecode = value; } }

        /// <summary>
        /// 邀请类型
        /// </summary>
        private string _activatetype;
        public string ActivateType { get { return _activatetype; } set { _activatetype = value; } }

        /// <summary>
        /// 用户来源
        /// </summary>
        private string _userfrom;
        public string UserFrom { get { return _userfrom; } set { _userfrom = value; } }

        /// <summary>
        /// 用户卡号
        /// </summary>
        private string _creditcardno;
        public string CreditcardNo { get { return _creditcardno; } set { _creditcardno = value; } }

        /// <summary>
        /// 用户注册密码
        /// </summary>
        private string _password;
        public string Password { get { return _password; } set { _password = value; } }

        /// <summary>
        /// 邮箱是否验证
        /// </summary>
        private short _isemailverified;
        public short IsEmailVerified { get { return _isemailverified; } set { _isemailverified = value; } }


        /// <summary>
        /// 手机是否验证
        /// </summary>
        private short _ismobileverified;
        public short IsMobileVerified { get { return _ismobileverified; } set { _ismobileverified = value; } }


        /// <summary>
        /// 用户注册确认密码
        /// </summary>
        private string _cpassword;
        public string CPassword { get { return _cpassword; } set { _cpassword = value; } }

        private string _introducer;
        /// <summary>
        /// 邀请人
        /// </summary>
        public string Introducer { get { return _introducer; } set { _introducer = value; } }

        private int _inviteCount;
        /// <summary>
        /// 邀请数
        /// </summary>
        public int InviteCount { get { return _inviteCount; } set { _inviteCount = value; } }

        /// <summary>
        /// 用户等级
        /// </summary>
        private string _levelno;
        public string LevelNo { get { return _levelno; } set { _levelno = value; } }

        /// <summary>
        /// 会员卡激活时使用 , cardPrefix + cardNo
        /// </summary>
        private string _membercardno;
        public string MemberCardNo { get { return _membercardno; } set { _membercardno = value; } }

        /// <summary>
        /// 尚品网的会员ID
        /// </summary>
        private string _userid;
        public string UserId { get { return _userid; } set { _userid = value; } }


        /// <summary>
        /// 快登项目的ID ，如 , 支付宝快登 , 则返回用户在支付宝里的AlipayUID
        /// </summary>
        private string _quickid;
        public string QuickID { get { return _quickid; } set { _quickid = value; } }


        /// <summary>
        /// 51返利比较特殊需要验证此值，写入用户表，用于第二次登录时做验证。如有解绑功能，解绑即是清除本字段值。
        /// </summary>
        private string _securitySalt=string.Empty;
        public string SecuritySalt { get { return _securitySalt; } set { _securitySalt = value; } }

        #endregion
    }
}
