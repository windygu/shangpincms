using System;

namespace Shangpin.Entity.Payment
{
    public class BankActivityInfo
    {
        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName{get;set;}
        /// <summary>
        /// 银行简称icbc
        /// </summary>
        public string BankEnName{get;set;}
        /// <summary>
        ///支付提示名称    
        /// </summary>
        public string PayTipsName { get; set; }   
        /// <summary>
        /// 邀请码
        /// </summary>
        public string InviteCode{get;set;}
        /// <summary>
        /// 支付方式选项
        /// </summary>
        public string PayModel{get;set;}
        /// <summary>
        /// 用户来源 （排他使用）
        /// </summary>
        public string IsUserFrom { get; set; }
        /// <summary>
        /// 支付方式（网银支付，信用卡支付，货到付款等等）支付通道ID（可扩展）0网银通道,1快捷通道,-1平台通道
        /// </summary>
        public string PayType { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime DateBegin{get;set;}
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime DateEnd{get;set;}
        /// <summary>
        /// 是否为唯一支付方式
        /// </summary>
        public Boolean IsOnlyPayType{ get; set; }
        
    }
}
