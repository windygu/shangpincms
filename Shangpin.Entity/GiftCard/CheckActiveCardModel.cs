using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.GiftCard
{
    public class CheckActiveCardModel
    {
        public string GiftCardNo{get;set;}
        public string PassWord{get;set;}
        public string MobileNo { get; set; }
        public string VerifyCode{get;set;}
        public string MobileVerifyCode { get; set; }
        public string PayPassWord { get; set; }
        /// <summary>
        /// 绑定类型 0个人中心绑定1领取电子卡直接绑定
        /// </summary>
        public short CheckFlag{get;set;}
        
        /// <summary>
        /// 1:卡号密码基本信息验证2手机验证3支付密码验证
        /// </summary>
        public short CheckStepFlag { get; set; }       
    }
}
