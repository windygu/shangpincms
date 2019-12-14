using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Shangpin.Framework.Common;

namespace Shangpin.Entity.Register
{
    public class RegisterModel
    {
        [Display(Name = "Email地址")]
        [Required(ErrorMessage = "请填写您的常用邮箱")]
        [RegularExpression(RegexCollectionHelper.EmailRegex, ErrorMessage = "请输入正确的邮箱地址")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "密码", Description = "密码由6-20位字符组成")]
        [Required(ErrorMessage = "请输入密码")]
        [StringLength(20, ErrorMessage = "密码长度只能在6-20之间", MinimumLength = 6)]
        public string Pwd { get; set; }

        [Display(Name = "性别")]
        [Range(-1, 1, ErrorMessage = "请选择性别")]
        public short Gender { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "密码", Description = "密码由6-20位字符组成")]
        [Required(ErrorMessage = "请输入密码")]
        [StringLength(20, ErrorMessage = "密码长度只能在6-20之间", MinimumLength = 6)]
        [Compare("Pwd", ErrorMessage = "密码必须相同")]
        public string RePwd { get; set; }

        [Display(Name = "同意注册条款")]
        [Range(0, 1, ErrorMessage = "请选择同意注册条款")]
        public bool Agree { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 页面重定向
        /// </summary>
        public string ReturnUrl { get; set; }
        public string VerifyCode { get; set; }
    }

    public class UserRegisterQueueMsg
    {
        public string UserId { get; set; }
    }
}
