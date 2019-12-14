using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common.Dapper;

namespace Shangpin.Entity.User
{
    public class InviteInfo
    {
        public SWfsInviteAccount InviteAccount { get; set; }
        public decimal AlreadyConsumeAmount { get; set; }
        public decimal NextRegAmount { get; set; }
        public decimal NextOrderAmount { get; set; }
        public string InviteCode { get; set; }
        public byte[] QRImagesFile { get; set; }
        public Page<InviteCashRecord> CashList { get; set; }
        public Page<InviteRecord> InviteList { get; set; }
        
    }
    public class InviteCodeInfo
    { 
        public string    SerialNo {get;set;}
        public string    SerialNoGuid {get;set;}
        public string    TrueName {get;set;}
        public string    InviteCode {get;set;}
        public string    EmailAddress {get;set;}
        public string    LoginName {get;set;}
        public string    UserId { get; set; }
        
    }
    public class InviteCashRecord
    {
        public string UserId { get; set; }
        public string MailAddress  {get;set;}
        public DateTime Date { get; set; }
        public decimal Amount{get;set;}
        public int sType { get; set; }
    }
    public class InviteRecord
    {
        public string InviteUserId { get; set; }
        public string MailAddress  {get;set;}
        public DateTime RegDate { get; set; }
        public decimal RegAmount{get;set;}
    }
    public class RegSendCashAccountInfo
    {
        public string UserId { get; set; }
        public DateTime DateCreate { get; set; }
        public short IsMobileVerified { get; set; }
    }
    public class FirstOrderInfo
    { 
        //t2.IntroducerId,t1.UserId,t1.OrderNo
        public string IntroducerId { get; set; }
        public string UserId { get; set; }
        public string OrderNo { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime RegDate { get; set; }
        public short IsMobileVerified { get; set; }
    }
}
